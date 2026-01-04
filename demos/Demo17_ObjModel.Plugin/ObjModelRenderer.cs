using AngleBridge.Sdk.HotReload;
using AngleBridge.Sdk.Interop;
using AngleBridge.Sdk.Runtime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Demo17_ObjModel.Plugin
{
    public sealed class ObjModelRenderer : IRenderer, IRendererWithHostContext
    {
        private AngleGl? _gl;
        private AngleBuffer? _vertexBuffer;
        private AngleBuffer? _indexBuffer;
        private AngleVertexArray? _vao;
        private ShaderProgramReloader? _programReloader;
        
        // Shader locations
        private int _positionLoc;
        private int _normalLoc;
        private int _modelLoc;
        private int _viewLoc;
        private int _projLoc;

        private bool _initialized;
        private int _indexCount;
        private Vector3 _meshCenter;
        private float _meshScale = 1.0f;

        private const string ObjFileName = "OPBR_Ace.obj";
        private readonly Vector3 _viewTranslate = new Vector3(0f, -0.3f, -3.5f);

        public void SetHostContext(RendererHostContext context)
        {
            // Simple shader reloading setup
            var roots = new[] { context.PluginDirectory, AppContext.BaseDirectory, Directory.GetCurrentDirectory() };
            _programReloader = new ShaderProgramReloader(
                Path.Combine("Shaders", "ObjModel.vert.glsl"),
                Path.Combine("Shaders", "ObjModel.frag.glsl"),
                roots);
        }

        public void Draw(AngleGl gl, int width, int height)
        {
            if (_gl == null) _gl = gl;
            if (!_initialized) Initialize(gl);

            // Shader Hot Reload
            var reloader = _programReloader!;
            if (reloader.TryReload(gl))
            {
                UpdateLocations(gl, reloader.Current);
            }

            if (reloader.LastError != null)
            {
                // Optionally log error, but keep running previous version
            }

            // Render
            gl.State.Viewport(0, 0, width, height);
            gl.State.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            gl.State.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            gl.State.Enable(EnableCap.DepthTest);

            gl.Programs.UseProgram(reloader.Current);

            BindVao(gl);
            UpdateMatrices(gl, width, height);

            gl.Draw.DrawElements(PrimitiveType.Triangles, _indexCount, IndexElementType.UnsignedShort, 0);
        }

        private void Initialize(AngleGl gl)
        {
            var reloader = _programReloader ?? throw new InvalidOperationException("Loader not set");
            reloader.Load(gl);
            UpdateLocations(gl, reloader.Current);

            // Load Mesh
            var objPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ObjFileName);
            // Fallback search if not in bin
            if (!File.Exists(objPath))
            {
                // Try to find it in the project folder (for dev)
                var current = new DirectoryInfo(Directory.GetCurrentDirectory());
                while (current != null)
                {
                     var candidate = Path.Combine(current.FullName, "demos", "Demo17_ObjModel.Plugin", ObjFileName);
                     if (File.Exists(candidate)) { objPath = candidate; break; }
                     current = current.Parent;
                }
            }

            if (!File.Exists(objPath)) throw new FileNotFoundException($"Cannot find {ObjFileName}");

            var mesh = SimpleObjLoader.Load(objPath);
            _meshCenter = mesh.Center;
            _meshScale = mesh.Scale;
            _indexCount = mesh.Indices.Length;

            // Buffers
            _vertexBuffer = gl.Buffers.CreateBufferObject();
            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
            gl.Buffers.BufferData(BufferTarget.ArrayBuffer, mesh.Vertices, BufferUsage.StaticDraw);

            _indexBuffer = gl.Buffers.CreateBufferObject();
            gl.Buffers.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
            gl.Buffers.BufferData(BufferTarget.ElementArrayBuffer, mesh.Indices, BufferUsage.StaticDraw);

            // VAO
            _vao = gl.VertexArrays.CreateVertexArrayObject();
            gl.VertexArrays.BindVertexArray(_vao);
            
            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
            gl.Buffers.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);

            // Attribute setup (Position: 0, Normal: 1)
            // Stride = 6 floats (3 pos + 3 normal)
            int stride = 6 * sizeof(float);
            
            gl.VertexArrays.EnableVertexAttribArray(_positionLoc);
            gl.VertexArrays.VertexAttribPointer(_positionLoc, 3, VertexAttribType.Float, false, stride, 0);
            
            gl.VertexArrays.EnableVertexAttribArray(_normalLoc);
            gl.VertexArrays.VertexAttribPointer(_normalLoc, 3, VertexAttribType.Float, false, stride, 3 * sizeof(float));

            _initialized = true;
        }

        private void BindVao(AngleGl gl)
        {
            gl.VertexArrays.BindVertexArray(_vao!);
        }

        private void UpdateLocations(AngleGl gl, AngleProgram program)
        {
            _positionLoc = gl.Programs.GetAttribLocation(program, "a_position");
            _normalLoc = gl.Programs.GetAttribLocation(program, "a_normal");
            
            _modelLoc = gl.Programs.GetUniformLocation(program, "u_model");
            _viewLoc = gl.Programs.GetUniformLocation(program, "u_view");
            _projLoc = gl.Programs.GetUniformLocation(program, "u_proj");
        }

        private void UpdateMatrices(AngleGl gl, int width, int height)
        {
            float aspect = (float)width / height;
            
            // Proj: FOV 60 deg
            var proj = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 3.0f, aspect, 0.1f, 100.0f);
            
            // View
            var view = Matrix4x4.CreateTranslation(_viewTranslate);

            // Model: Translate Center -> Scale -> Rotate X -> Rotate Y
            var time = (float)(Environment.TickCount % 100000) / 1000.0f;
            
            var tCenter = Matrix4x4.CreateTranslation(-_meshCenter);
            var scale = Matrix4x4.CreateScale(_meshScale);
            // var rotX = Matrix4x4.CreateRotationX(-0.35f); // Remove unwanted tilt
            var rotY = Matrix4x4.CreateRotationY(time);

            // Order: v * T_center * Scale * RotY
            var model = tCenter * scale * rotY;

            // Upload (transpose=false because System.Numerics memory layout matches GL column-major expectations)
            UploadMatrix(gl, _projLoc, proj);
            UploadMatrix(gl, _viewLoc, view);
            UploadMatrix(gl, _modelLoc, model);
        }

        private void UploadMatrix(AngleGl gl, int loc, Matrix4x4 m)
        {
            // System.Numerics.Matrix4x4 memory layout:
            // Row 1: M11, M12, M13, M14
            // ...
            // Row 4: M41, M42, M43, M44 (Translation)
            
            // OpenGL Expects Column-Major memory layout:
            // Col 1: m00, m01, m02, m03
            // ...
            // Col 4: m30, m31, m32, m33 (Translation)
            
            // So System.Numerics Row 1 maps to OpenGL Col 1.
            // We should send data as-is (transpose=false).

            float[] data = new float[] 
            {
                m.M11, m.M12, m.M13, m.M14,
                m.M21, m.M22, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.M41, m.M42, m.M43, m.M44
            };
            
            gl.Programs.UniformMatrix4fv(loc, false, data);
        }

        public void Dispose()
        {
            _vao?.Dispose();
            _vertexBuffer?.Dispose();
            _indexBuffer?.Dispose();
            _programReloader?.Dispose();
            _initialized = false;
        }

        // --- Simple OBJ Loader ---
        private static class SimpleObjLoader
        {
            public class MeshData
            {
                public float[] Vertices = Array.Empty<float>();
                public ushort[] Indices = Array.Empty<ushort>();
                public Vector3 Center;
                public float Scale;
            }

            public static MeshData Load(string path)
            {
                var positions = new List<Vector3>();
                var normals = new List<Vector3>();
                var resultVerts = new List<float>(); // Flattened: x,y,z, nx,ny,nz
                var resultIndices = new List<ushort>();
                
                // Cache to avoid duplicates: "v_idx/n_idx" -> new_index
                var cache = new Dictionary<string, ushort>();

                foreach (var line in File.ReadLines(path))
                {
                    if (line.StartsWith("v "))
                    {
                        var parts = ParseFloats(line);
                        positions.Add(new Vector3(parts[0], parts[1], parts[2]));
                    }
                    else if (line.StartsWith("vn "))
                    {
                        var parts = ParseFloats(line);
                        normals.Add(new Vector3(parts[0], parts[1], parts[2]));
                    }
                    else if (line.StartsWith("f "))
                    {
                        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        // Triangulate fan
                        for (int i = 2; i < parts.Length - 1; i++)
                        {
                            AddIndex(parts[1], positions, normals, resultVerts, resultIndices, cache);
                            AddIndex(parts[i], positions, normals, resultVerts, resultIndices, cache);
                            AddIndex(parts[i + 1], positions, normals, resultVerts, resultIndices, cache);
                        }
                    }
                }

                // Calculate bounds
                var min = new Vector3(float.MaxValue);
                var max = new Vector3(float.MinValue);
                for(int i=0; i<resultVerts.Count; i+=6)
                {
                    var p = new Vector3(resultVerts[i], resultVerts[i+1], resultVerts[i+2]);
                    min = Vector3.Min(min, p);
                    max = Vector3.Max(max, p);
                }
                var center = (min + max) * 0.5f;
                var size = max - min;
                var maxDim = MathF.Max(size.X, MathF.Max(size.Y, size.Z));
                var scale = maxDim > 0 ? 1.0f / maxDim * 2.0f : 1.0f; // Scale to fit approx unit box

                return new MeshData 
                { 
                    Vertices = resultVerts.ToArray(), 
                    Indices = resultIndices.ToArray(),
                    Center = center,
                    Scale = scale
                };
            }

            private static void AddIndex(string token, List<Vector3> pos, List<Vector3> norm, List<float> verts, List<ushort> indices, Dictionary<string, ushort> cache)
            {
                if (cache.TryGetValue(token, out var idx))
                {
                    indices.Add(idx);
                    return;
                }

                var sub = token.Split('/');
                int pIdx = int.Parse(sub[0]) - 1;
                int nIdx = sub.Length > 2 && int.TryParse(sub[2], out int n) ? n - 1 : -1;

                var p = pos[pIdx];
                var nVec = (nIdx >= 0 && nIdx < norm.Count) ? norm[nIdx] : Vector3.Zero;

                verts.Add(p.X); verts.Add(p.Y); verts.Add(p.Z);
                verts.Add(nVec.X); verts.Add(nVec.Y); verts.Add(nVec.Z);

                ushort newIdx = (ushort)((verts.Count / 6) - 1);
                indices.Add(newIdx);
                cache[token] = newIdx;
            }

            private static float[] ParseFloats(string line)
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new[] 
                { 
                    float.Parse(parts[1], CultureInfo.InvariantCulture),
                    float.Parse(parts[2], CultureInfo.InvariantCulture),
                    float.Parse(parts[3], CultureInfo.InvariantCulture)
                };
            }
        }
    }
}
