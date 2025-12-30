using AngleBridge.Sdk.HotReload;
using AngleBridge.Sdk.Interop;
using AngleBridge.Sdk.Runtime;
using System;
using System.IO;

namespace Demo6_StencilOperations.Plugin
{
    public sealed class StencilOperationsRenderer : IRenderer, IRendererWithHostContext
    {
        private AngleGl? _gl;
        private AngleBuffer? _vertexBuffer;
        private AngleBuffer? _indexBuffer;
        private AngleVertexArray? _vao;
        private int _positionLoc;
        private int _colorLoc;
        private bool _useVao;
        private bool _initialized;
        private ShaderProgramReloader? _programReloader;
        private Exception? _lastReloadError;

        private static readonly float[] s_vertices =
        {
            -0.75f,  0.25f, 0.50f,  // Quad #0
            -0.25f,  0.25f, 0.50f,
            -0.25f,  0.75f, 0.50f,
            -0.75f,  0.75f, 0.50f,

             0.25f,  0.25f, 0.90f,  // Quad #1
             0.75f,  0.25f, 0.90f,
             0.75f,  0.75f, 0.90f,
             0.25f,  0.75f, 0.90f,

            -0.75f, -0.75f, 0.50f,  // Quad #2
            -0.25f, -0.75f, 0.50f,
            -0.25f, -0.25f, 0.50f,
            -0.75f, -0.25f, 0.50f,

             0.25f, -0.75f, 0.50f,  // Quad #3
             0.75f, -0.75f, 0.50f,
             0.75f, -0.25f, 0.50f,
             0.25f, -0.25f, 0.50f,

            -1.00f, -1.00f, 0.00f,  // Big Quad
             1.00f, -1.00f, 0.00f,
             1.00f,  1.00f, 0.00f,
            -1.00f,  1.00f, 0.00f,
        };

        private static readonly ushort[] s_indices =
        {
            0, 1, 2, 0, 2, 3,        // Quad #0
            4, 5, 6, 4, 6, 7,        // Quad #1
            8, 9, 10, 8, 10, 11,     // Quad #2
            12, 13, 14, 12, 14, 15,  // Quad #3
            16, 17, 18, 16, 18, 19,  // Big Quad
        };

        private static readonly int[] s_indexOffsetsBytes =
        {
            0,
            12,
            24,
            36,
            48,
        };

        private static readonly float[,] s_colors =
        {
            { 1.0f, 0.0f, 0.0f, 1.0f },
            { 0.0f, 1.0f, 0.0f, 1.0f },
            { 0.0f, 0.0f, 1.0f, 1.0f },
            { 1.0f, 1.0f, 0.0f, 1.0f },
        };

        public void Draw(AngleGl gl, int width, int height)
        {
            if (gl == null) throw new ArgumentNullException(nameof(gl));
            if (_gl == null)
                _gl = gl;
            else if (!ReferenceEquals(_gl, gl))
                throw new InvalidOperationException("Renderer cannot be shared across different AngleGl instances.");

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

            gl.Viewport(0, 0, width, height);
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            gl.UseProgram(programReloader.Current);

            if (_useVao)
            {
                gl.BindVertexArray(_vao ?? throw new InvalidOperationException("Vertex array not initialized."));
            }
            else
            {
                BindGeometry(gl);
            }

            gl.StencilMask(0xFF);

            // Test 0
            gl.StencilFunc(StencilFunction.Less, 0x7, 0x3);
            gl.StencilOp(StencilOp.Replace, StencilOp.Decr, StencilOp.Decr);
            gl.DrawElements(PrimitiveType.Triangles, 6, IndexElementType.UnsignedShort, s_indexOffsetsBytes[0]);

            // Test 1
            gl.StencilFunc(StencilFunction.Greater, 0x3, 0x3);
            gl.StencilOp(StencilOp.Keep, StencilOp.Decr, StencilOp.Keep);
            gl.DrawElements(PrimitiveType.Triangles, 6, IndexElementType.UnsignedShort, s_indexOffsetsBytes[1]);

            // Test 2
            gl.StencilFunc(StencilFunction.Equal, 0x1, 0x3);
            gl.StencilOp(StencilOp.Keep, StencilOp.Incr, StencilOp.Incr);
            gl.DrawElements(PrimitiveType.Triangles, 6, IndexElementType.UnsignedShort, s_indexOffsetsBytes[2]);

            // Test 3
            gl.StencilFunc(StencilFunction.Equal, 0x2, 0x1);
            gl.StencilOp(StencilOp.Invert, StencilOp.Keep, StencilOp.Keep);
            gl.DrawElements(PrimitiveType.Triangles, 6, IndexElementType.UnsignedShort, s_indexOffsetsBytes[3]);

            var stencilValues = new int[4] { 0x7, 0x0, 0x2, 0x0 };
            var bits = gl.GetInteger(GetIntegerParameter.StencilBits);
            var mask = bits >= 1 && bits < 31 ? ((1 << bits) - 1) : unchecked((int)0xFF);
            stencilValues[3] = ~((mask & 0x1)) & 0xFF;

            gl.StencilMask(0x0);

            var colorLoc = _colorLoc;
            for (var i = 0; i < stencilValues.Length; i++)
            {
                gl.StencilFunc(StencilFunction.Equal, stencilValues[i], 0xFF);
                gl.Uniform4f(colorLoc, s_colors[i, 0], s_colors[i, 1], s_colors[i, 2], s_colors[i, 3]);
                gl.DrawElements(PrimitiveType.Triangles, 6, IndexElementType.UnsignedShort, s_indexOffsetsBytes[4]);
            }

            gl.StencilMask(0xFF);
        }

        public void Dispose()
        {
            if (_vao != null) _vao.Dispose();
            if (_indexBuffer != null) _indexBuffer.Dispose();
            if (_vertexBuffer != null) _vertexBuffer.Dispose();
            if (_programReloader != null) _programReloader.Dispose();

            _vao = null;
            _indexBuffer = null;
            _vertexBuffer = null;
            _gl = null;
            _initialized = false;
            _useVao = false;
            _programReloader = null;
            _lastReloadError = null;
        }

        private void Initialize(AngleGl gl)
        {
            EnsureProgramReloader();
            var reloader = _programReloader ?? throw new InvalidOperationException("Program reloader not initialized.");
            reloader.Load(gl);
            UpdateProgramBindings(gl, reloader.Current);

            _vertexBuffer = gl.CreateBufferObject();
            gl.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
            gl.BufferData(BufferTarget.ArrayBuffer, s_vertices, BufferUsage.StaticDraw);

            _indexBuffer = gl.CreateBufferObject();
            gl.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
            gl.BufferData(BufferTarget.ElementArrayBuffer, s_indices, BufferUsage.StaticDraw);

            try
            {
                _vao = gl.CreateVertexArrayObject();
                _useVao = true;
                gl.BindVertexArray(_vao);
                BindGeometry(gl);
            }
            catch (NotSupportedException)
            {
                _useVao = false;
                _vao = null;
            }

            gl.ClearColor(0f, 0f, 0f, 1f);
            gl.ClearStencil(0x1);
            gl.ClearDepth(0.75f);
            gl.Enable(EnableCap.DepthTest);
            gl.Enable(EnableCap.StencilTest);

            _initialized = true;
        }

        private void UpdateProgramBindings(AngleGl gl, AngleProgram program)
        {
            _positionLoc = gl.GetAttribLocation(program, "a_position");
            _colorLoc = gl.GetUniformLocation(program, "u_color");
            if (_positionLoc < 0 || _colorLoc < 0)
                throw new InvalidOperationException("Failed to resolve shader bindings.");

            if (_vao != null)
                BindGeometry(gl);
        }

        private void BindGeometry(AngleGl gl)
        {
            gl.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer ?? throw new InvalidOperationException("Vertex buffer not initialized."));
            gl.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer ?? throw new InvalidOperationException("Index buffer not initialized."));
            gl.EnableVertexAttribArray(_positionLoc);
            gl.VertexAttribPointer(_positionLoc, 3, VertexAttribType.Float, normalized: false, stride: 3 * sizeof(float), offsetBytes: 0);
        }

        public void SetHostContext(RendererHostContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var roots = BuildShaderRoots(context, "Demo6_StencilOperations.Plugin.csproj");
            _programReloader = new ShaderProgramReloader(
                Path.Combine("Shaders", "StencilOperations.vert.glsl"),
                Path.Combine("Shaders", "StencilOperations.frag.glsl"),
                roots);
        }

        private void EnsureProgramReloader()
        {
            if (_programReloader != null) return;
            var roots = new[]
            {
                AppContext.BaseDirectory,
                Directory.GetCurrentDirectory(),
            };
            _programReloader = new ShaderProgramReloader(
                Path.Combine("Shaders", "StencilOperations.vert.glsl"),
                Path.Combine("Shaders", "StencilOperations.frag.glsl"),
                roots);
        }

        private static string[] BuildShaderRoots(RendererHostContext context, string projectFileName)
        {
            var roots = new[]
            {
                context.PluginDirectory,
                AppContext.BaseDirectory,
                Directory.GetCurrentDirectory(),
            };

            var projectRoot = TryGetProjectRootFromPluginOutput(context.PluginDirectory, projectFileName);
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

        private static string? TryGetProjectRootFromPluginOutput(string pluginDir, string projectFileName)
        {
            try
            {
                var di = new DirectoryInfo(pluginDir);
                for (var i = 0; i < 5 && di != null; i++)
                {
                    var candidate = Path.Combine(di.FullName, projectFileName);
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
    }
}
