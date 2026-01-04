using AngleBridge.Sdk.HotReload;
using AngleBridge.Sdk.Interop;
using AngleBridge.Sdk.Runtime;
using System;
using System.Diagnostics;
using System.IO;

namespace Demo18_PostSubBuffer.Plugin
{
    public sealed class PostSubBufferRenderer : IRenderer, IRendererWithHostContext, IRendererWithPresent
    {
        private bool _initialized;
        private AngleGl? _gl;

        private AngleBuffer? _positionBuffer;
        private AngleBuffer? _texcoordBuffer;
        private AngleBuffer? _indexBuffer;
        private AngleVertexArray? _vao;

        private int _positionLoc;
        private int _texcoordLoc;
        private int _mvpLoc;
        private float _rotation;

        private readonly Stopwatch _timer = new Stopwatch();
        private double _prevTime;

        private ShaderProgramReloader? _programReloader;
        private Exception? _lastReloadError;

        private bool _didInitialClear;
        private bool _pendingInitialPresent;

        private const float Radius = 0.5f;

        private static readonly float[] s_positions =
        {
            -Radius, -Radius, -Radius,
            -Radius, -Radius, Radius,
            Radius, -Radius, Radius,
            Radius, -Radius, -Radius,
            -Radius, Radius, -Radius,
            -Radius, Radius, Radius,
            Radius, Radius, Radius,
            Radius, Radius, -Radius,
            -Radius, -Radius, -Radius,
            -Radius, Radius, -Radius,
            Radius, Radius, -Radius,
            Radius, -Radius, -Radius,
            -Radius, -Radius, Radius,
            -Radius, Radius, Radius,
            Radius, Radius, Radius,
            Radius, -Radius, Radius,
            -Radius, -Radius, -Radius,
            -Radius, -Radius, Radius,
            -Radius, Radius, Radius,
            -Radius, Radius, -Radius,
            Radius, -Radius, -Radius,
            Radius, -Radius, Radius,
            Radius, Radius, Radius,
            Radius, Radius, -Radius,
        };

        private static readonly float[] s_texcoords =
        {
            0.0f, 0.0f,
            0.0f, 1.0f,
            1.0f, 1.0f,
            1.0f, 0.0f,
            1.0f, 0.0f,
            1.0f, 1.0f,
            0.0f, 1.0f,
            0.0f, 0.0f,
            0.0f, 0.0f,
            0.0f, 1.0f,
            1.0f, 1.0f,
            1.0f, 0.0f,
            0.0f, 0.0f,
            0.0f, 1.0f,
            1.0f, 1.0f,
            1.0f, 0.0f,
            0.0f, 0.0f,
            0.0f, 1.0f,
            1.0f, 1.0f,
            1.0f, 0.0f,
            0.0f, 0.0f,
            0.0f, 1.0f,
            1.0f, 1.0f,
            1.0f, 0.0f,
        };

        private static readonly ushort[] s_indices =
        {
            0, 2, 1,
            0, 3, 2,
            4, 5, 6,
            4, 6, 7,
            8, 9, 10,
            8, 10, 11,
            12, 15, 14,
            12, 14, 13,
            16, 17, 18,
            16, 18, 19,
            20, 23, 22,
            20, 22, 21,
        };

        public void Draw(AngleGl gl, int width, int height)
        {
            if (gl == null) throw new ArgumentNullException(nameof(gl));
            if (_gl == null)
            {
                _gl = gl;
                _timer.Start();
            }
            else if (!ReferenceEquals(_gl, gl))
            {
                throw new InvalidOperationException("Renderer cannot be shared across different AngleGl instances.");
            }

            if (!_initialized)
                Initialize(gl);

            var programReloader = _programReloader ?? throw new InvalidOperationException("Program reloader not initialized.");
            if (programReloader.TryReload(gl))
                UpdateProgramBindings(gl, programReloader.Current);

            var reloadError = programReloader.LastError;
            if (reloadError != null && !ReferenceEquals(reloadError, _lastReloadError))
            {
                _lastReloadError = reloadError;
                Console.Error.WriteLine(reloadError);
            }

            if (!_didInitialClear)
            {
                gl.State.ClearColor(0.0f, 0.0f, 1.0f, 0.0f);
                gl.State.Clear(ClearBufferMask.ColorBufferBit);
                _pendingInitialPresent = true;
                _didInitialClear = true;

                gl.State.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
                gl.State.CullFace(CullFaceMode.Back);
                gl.State.Enable(EnableCap.CullFace);
                return;
            }

            UpdateMatrices(gl, width, height);

            gl.State.Viewport(0, 0, width, height);
            gl.State.Clear(ClearBufferMask.ColorBufferBit);
            gl.Programs.UseProgram(programReloader.Current);
            gl.VertexArrays.BindVertexArray(_vao ?? throw new InvalidOperationException("Vertex array not initialized."));
            gl.Draw.DrawElements(PrimitiveType.Triangles, s_indices.Length, IndexElementType.UnsignedShort, 0);
        }

        public void Present(AngleDevice device, int width, int height)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));

            if (_pendingInitialPresent)
            {
                device.Present();
                _pendingInitialPresent = false;
                return;
            }

            int w = width - 120;
            int h = height - 120;
            if (w <= 0 || h <= 0)
            {
                device.Present();
                return;
            }

            device.PostSubBuffer(60, 60, w, h);
        }

        public void SetHostContext(RendererHostContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var roots = BuildShaderRoots(context);
            _programReloader = new ShaderProgramReloader(
                Path.Combine("Shaders", "PostSubBuffer.vert.glsl"),
                Path.Combine("Shaders", "PostSubBuffer.frag.glsl"),
                roots);
        }

        public void Dispose()
        {
            if (_vao != null) _vao.Dispose();
            if (_indexBuffer != null) _indexBuffer.Dispose();
            if (_texcoordBuffer != null) _texcoordBuffer.Dispose();
            if (_positionBuffer != null) _positionBuffer.Dispose();
            if (_programReloader != null) _programReloader.Dispose();
            _programReloader = null;

            _vao = null;
            _indexBuffer = null;
            _texcoordBuffer = null;
            _positionBuffer = null;
            _initialized = false;
            _gl = null;
            _timer.Reset();
            _prevTime = 0.0;
            _rotation = 0.0f;
            _lastReloadError = null;
            _didInitialClear = false;
            _pendingInitialPresent = false;
        }

        private void Initialize(AngleGl gl)
        {
            var programReloader = _programReloader ?? throw new InvalidOperationException("Program reloader not initialized.");
            programReloader.Load(gl);
            UpdateProgramBindings(gl, programReloader.Current);

            _positionBuffer = gl.Buffers.CreateBufferObject();
            _texcoordBuffer = gl.Buffers.CreateBufferObject();
            _indexBuffer = gl.Buffers.CreateBufferObject();
            _vao = gl.VertexArrays.CreateVertexArrayObject();

            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _positionBuffer);
            gl.Buffers.BufferData(BufferTarget.ArrayBuffer, s_positions, BufferUsage.StaticDraw);

            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _texcoordBuffer);
            gl.Buffers.BufferData(BufferTarget.ArrayBuffer, s_texcoords, BufferUsage.StaticDraw);

            gl.Buffers.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
            gl.Buffers.BufferData(BufferTarget.ElementArrayBuffer, s_indices, BufferUsage.StaticDraw);
            SetupGeometry(gl);

            _rotation = 45.0f;
            _initialized = true;
        }

        private void UpdateProgramBindings(AngleGl gl, AngleProgram program)
        {
            _positionLoc = gl.Programs.GetAttribLocation(program, "a_position");
            _texcoordLoc = gl.Programs.GetAttribLocation(program, "a_texcoord");
            _mvpLoc = gl.Programs.GetUniformLocation(program, "u_mvpMatrix");

            if (_positionLoc < 0 || _texcoordLoc < 0 || _mvpLoc < 0)
                throw new InvalidOperationException("Failed to resolve shader attribute/uniform locations.");

            if (_vao != null && _positionBuffer != null && _texcoordBuffer != null && _indexBuffer != null)
                SetupGeometry(gl);
        }

        private void SetupGeometry(AngleGl gl)
        {
            var vao = _vao ?? throw new InvalidOperationException("Vertex array not initialized.");
            var positionBuffer = _positionBuffer ?? throw new InvalidOperationException("Position buffer not initialized.");
            var texcoordBuffer = _texcoordBuffer ?? throw new InvalidOperationException("Texcoord buffer not initialized.");
            var indexBuffer = _indexBuffer ?? throw new InvalidOperationException("Index buffer not initialized.");

            gl.VertexArrays.BindVertexArray(vao);

            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);
            gl.VertexArrays.VertexAttribPointer(_positionLoc, 3, VertexAttribType.Float, false, 0, 0);
            gl.VertexArrays.EnableVertexAttribArray(_positionLoc);

            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, texcoordBuffer);
            gl.VertexArrays.VertexAttribPointer(_texcoordLoc, 2, VertexAttribType.Float, false, 0, 0);
            gl.VertexArrays.EnableVertexAttribArray(_texcoordLoc);

            gl.Buffers.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
        }

        private void UpdateMatrices(AngleGl gl, int width, int height)
        {
            var elapsed = _timer.Elapsed.TotalSeconds;
            var dt = elapsed - _prevTime;
            _prevTime = elapsed;

            _rotation = (float)((_rotation + (dt * 40.0f)) % 360.0f);

            var aspect = height > 0 ? (float)width / height : 1.0f;
            var perspective = Matrix4.Perspective(60.0f, aspect, 1.0f, 20.0f);
            var model = Matrix4.Translate(new Vector3(0.0f, 0.0f, -2.0f)) *
                        Matrix4.Rotate(_rotation, new Vector3(1.0f, 0.0f, 1.0f));
            var view = Matrix4.Identity();
            var mvp = perspective * view * model;

            var programReloader = _programReloader ?? throw new InvalidOperationException("Program reloader not initialized.");
            gl.Programs.UseProgram(programReloader.Current);
            gl.Programs.UniformMatrix4fv(_mvpLoc, false, mvp.Data);
        }

        private static string[] BuildShaderRoots(RendererHostContext context)
        {
            var roots = new[]
            {
                context.PluginDirectory,
                AppContext.BaseDirectory,
                Directory.GetCurrentDirectory(),
            };

            var projectRoot = TryGetProjectRootFromPluginOutput(context.PluginDirectory);
            if (!string.IsNullOrWhiteSpace(projectRoot))
            {
                return new[]
                {
                    projectRoot,
                    context.PluginDirectory,
                    AppContext.BaseDirectory,
                    Directory.GetCurrentDirectory(),
                };
            }

            return roots;
        }

        private static string? TryGetProjectRootFromPluginOutput(string pluginDir)
        {
            try
            {
                var di = new DirectoryInfo(pluginDir);
                for (var i = 0; i < 5 && di != null; i++)
                {
                    var candidate = Path.Combine(di.FullName, "Demo18_PostSubBuffer.Plugin.csproj");
                    if (File.Exists(candidate))
                        return di.FullName;
                    di = di.Parent;
                }
            }
            catch
            {
            }

            return null;
        }

        private struct Vector3
        {
            public float X;
            public float Y;
            public float Z;

            public Vector3(float x, float y, float z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public Vector3 Normalized()
            {
                var len = MathF.Sqrt((X * X) + (Y * Y) + (Z * Z));
                if (len <= 0.0f) return new Vector3(0.0f, 0.0f, 0.0f);
                var inv = 1.0f / len;
                return new Vector3(X * inv, Y * inv, Z * inv);
            }
        }

        private struct Matrix4
        {
            public readonly float[] Data;

            private Matrix4(float[] data)
            {
                Data = data;
            }

            public static Matrix4 Identity()
            {
                return FromElements(
                    1.0f, 0.0f, 0.0f, 0.0f,
                    0.0f, 1.0f, 0.0f, 0.0f,
                    0.0f, 0.0f, 1.0f, 0.0f,
                    0.0f, 0.0f, 0.0f, 1.0f);
            }

            public static Matrix4 Rotate(float angle, Vector3 axis)
            {
                var u = axis.Normalized();
                var theta = angle * (MathF.PI / 180.0f);
                var cos = MathF.Cos(theta);
                var sin = MathF.Sin(theta);
                var oneMinus = 1.0f - cos;

                return FromElements(
                    cos + (u.X * u.X * oneMinus),
                    (u.X * u.Y * oneMinus) - (u.Z * sin),
                    (u.X * u.Z * oneMinus) + (u.Y * sin),
                    0.0f,
                    (u.Y * u.X * oneMinus) + (u.Z * sin),
                    cos + (u.Y * u.Y * oneMinus),
                    (u.Y * u.Z * oneMinus) - (u.X * sin),
                    0.0f,
                    (u.Z * u.X * oneMinus) - (u.Y * sin),
                    (u.Z * u.Y * oneMinus) + (u.X * sin),
                    cos + (u.Z * u.Z * oneMinus),
                    0.0f,
                    0.0f,
                    0.0f,
                    0.0f,
                    1.0f);
            }

            public static Matrix4 Translate(Vector3 t)
            {
                return FromElements(
                    1.0f, 0.0f, 0.0f, t.X,
                    0.0f, 1.0f, 0.0f, t.Y,
                    0.0f, 0.0f, 1.0f, t.Z,
                    0.0f, 0.0f, 0.0f, 1.0f);
            }

            public static Matrix4 Frustum(float l, float r, float b, float t, float n, float f)
            {
                return FromElements(
                    (2.0f * n) / (r - l), 0.0f, (r + l) / (r - l), 0.0f,
                    0.0f, (2.0f * n) / (t - b), (t + b) / (t - b), 0.0f,
                    0.0f, 0.0f, -(f + n) / (f - n), -(2.0f * f * n) / (f - n),
                    0.0f, 0.0f, -1.0f, 0.0f);
            }

            public static Matrix4 Perspective(float fovY, float aspectRatio, float nearZ, float farZ)
            {
                var frustumHeight = MathF.Tan((fovY / 360.0f) * MathF.PI) * nearZ;
                var frustumWidth = frustumHeight * aspectRatio;
                return Frustum(-frustumWidth, frustumWidth, -frustumHeight, frustumHeight, nearZ, farZ);
            }

            public static Matrix4 operator *(Matrix4 a, Matrix4 b)
            {
                return FromElements(
                    a.Data[0] * b.Data[0] + a.Data[4] * b.Data[1] + a.Data[8] * b.Data[2] + a.Data[12] * b.Data[3],
                    a.Data[0] * b.Data[4] + a.Data[4] * b.Data[5] + a.Data[8] * b.Data[6] + a.Data[12] * b.Data[7],
                    a.Data[0] * b.Data[8] + a.Data[4] * b.Data[9] + a.Data[8] * b.Data[10] + a.Data[12] * b.Data[11],
                    a.Data[0] * b.Data[12] + a.Data[4] * b.Data[13] + a.Data[8] * b.Data[14] + a.Data[12] * b.Data[15],
                    a.Data[1] * b.Data[0] + a.Data[5] * b.Data[1] + a.Data[9] * b.Data[2] + a.Data[13] * b.Data[3],
                    a.Data[1] * b.Data[4] + a.Data[5] * b.Data[5] + a.Data[9] * b.Data[6] + a.Data[13] * b.Data[7],
                    a.Data[1] * b.Data[8] + a.Data[5] * b.Data[9] + a.Data[9] * b.Data[10] + a.Data[13] * b.Data[11],
                    a.Data[1] * b.Data[12] + a.Data[5] * b.Data[13] + a.Data[9] * b.Data[14] + a.Data[13] * b.Data[15],
                    a.Data[2] * b.Data[0] + a.Data[6] * b.Data[1] + a.Data[10] * b.Data[2] + a.Data[14] * b.Data[3],
                    a.Data[2] * b.Data[4] + a.Data[6] * b.Data[5] + a.Data[10] * b.Data[6] + a.Data[14] * b.Data[7],
                    a.Data[2] * b.Data[8] + a.Data[6] * b.Data[9] + a.Data[10] * b.Data[10] + a.Data[14] * b.Data[11],
                    a.Data[2] * b.Data[12] + a.Data[6] * b.Data[13] + a.Data[10] * b.Data[14] + a.Data[14] * b.Data[15],
                    a.Data[3] * b.Data[0] + a.Data[7] * b.Data[1] + a.Data[11] * b.Data[2] + a.Data[15] * b.Data[3],
                    a.Data[3] * b.Data[4] + a.Data[7] * b.Data[5] + a.Data[11] * b.Data[6] + a.Data[15] * b.Data[7],
                    a.Data[3] * b.Data[8] + a.Data[7] * b.Data[9] + a.Data[11] * b.Data[10] + a.Data[15] * b.Data[11],
                    a.Data[3] * b.Data[12] + a.Data[7] * b.Data[13] + a.Data[11] * b.Data[14] + a.Data[15] * b.Data[15]);
            }

            private static Matrix4 FromElements(
                float m00, float m01, float m02, float m03,
                float m10, float m11, float m12, float m13,
                float m20, float m21, float m22, float m23,
                float m30, float m31, float m32, float m33)
            {
                var data = new float[16];
                data[0] = m00;
                data[4] = m01;
                data[8] = m02;
                data[12] = m03;
                data[1] = m10;
                data[5] = m11;
                data[9] = m12;
                data[13] = m13;
                data[2] = m20;
                data[6] = m21;
                data[10] = m22;
                data[14] = m23;
                data[3] = m30;
                data[7] = m31;
                data[11] = m32;
                data[15] = m33;
                return new Matrix4(data);
            }
        }
    }
}
