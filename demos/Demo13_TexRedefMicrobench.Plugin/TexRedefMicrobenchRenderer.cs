using AngleBridge.Sdk.HotReload;
using AngleBridge.Sdk.Interop;
using AngleBridge.Sdk.Runtime;
using System;
using System.Diagnostics;
using System.IO;

namespace Demo13_TexRedefMicrobench.Plugin
{
    public sealed class TexRedefMicrobenchRenderer : IRenderer, IRendererWithHostContext
    {
        private const int WarmupFrames = 5;
        private const int InitialSize = 256;
        private const int RedefineSize = 512;

        private AngleGl? _gl;
        private AngleBuffer? _vertexBuffer;
        private AngleBuffer? _indexBuffer;
        private AngleTexture? _resizeTexture;
        private AngleTexture? _newTexture;
        private ShaderProgramReloader? _programReloader;
        private Exception? _lastReloadError;
        private byte[]? _pixelsResize;
        private byte[]? _pixelsNew;
        private int _positionLoc;
        private int _texCoordLoc;
        private int _samplerLoc;
        private int _frameCount;
        private bool _initialized;
        private bool _ranBenchmark;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        private static readonly float[] s_vertices =
        {
            // posX,  posY, posZ,   u,   v
            -0.5f,  0.5f,  0.0f,   0.0f, 0.0f,
            -0.5f, -0.5f,  0.0f,   0.0f, 1.0f,
             0.5f, -0.5f,  0.0f,   1.0f, 1.0f,
             0.5f,  0.5f,  0.0f,   1.0f, 0.0f,
        };

        private static readonly ushort[] s_indices = { 0, 1, 2, 0, 2, 3 };

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

            gl.State.Viewport(0, 0, width, height);
            gl.State.Clear(ClearBufferMask.ColorBufferBit);
            gl.Programs.UseProgram(programReloader.Current);
            BindGeometry(gl);

            gl.Textures.ActiveTexture(TextureUnit.Texture0);
            gl.Textures.BindTexture(TextureTarget.Texture2D, _resizeTexture ?? throw new InvalidOperationException("Texture not initialized."));
            gl.Programs.Uniform1i(_samplerLoc, 0);

            if (!_ranBenchmark && _frameCount >= WarmupFrames)
            {
                RunBenchmark(gl);
                _ranBenchmark = true;
            }
            else
            {
                DrawQuad(gl);
            }

            _frameCount++;
        }

        public void Dispose()
        {
            if (_resizeTexture != null) _resizeTexture.Dispose();
            if (_newTexture != null) _newTexture.Dispose();
            if (_indexBuffer != null) _indexBuffer.Dispose();
            if (_vertexBuffer != null) _vertexBuffer.Dispose();
            if (_programReloader != null) _programReloader.Dispose();

            _resizeTexture = null;
            _newTexture = null;
            _indexBuffer = null;
            _vertexBuffer = null;
            _programReloader = null;
            _gl = null;
            _initialized = false;
            _ranBenchmark = false;
            _frameCount = 0;
            _lastReloadError = null;
            _pixelsResize = null;
            _pixelsNew = null;
        }

        public void SetHostContext(RendererHostContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var roots = BuildShaderRoots(context, "Demo13_TexRedefMicrobench.Plugin.csproj");
            _programReloader = new ShaderProgramReloader(
                Path.Combine("Shaders", "TexRedefMicrobench.vert.glsl"),
                Path.Combine("Shaders", "TexRedefMicrobench.frag.glsl"),
                roots);
        }

        private void Initialize(AngleGl gl)
        {
            EnsureProgramReloader();
            var reloader = _programReloader ?? throw new InvalidOperationException("Program reloader not initialized.");
            reloader.Load(gl);
            UpdateProgramBindings(gl, reloader.Current);

            CreateBuffers(gl);
            CreatePixelData();
            CreateTextures(gl);

            gl.State.ClearColor(0f, 0f, 0f, 0f);
            _initialized = true;
        }

        private void CreateBuffers(AngleGl gl)
        {
            _vertexBuffer = gl.Buffers.CreateBufferObject();
            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
            gl.Buffers.BufferData(BufferTarget.ArrayBuffer, s_vertices, BufferUsage.StaticDraw);

            _indexBuffer = gl.Buffers.CreateBufferObject();
            gl.Buffers.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
            gl.Buffers.BufferData(BufferTarget.ElementArrayBuffer, s_indices, BufferUsage.StaticDraw);
        }

        private void CreatePixelData()
        {
            var total = RedefineSize * RedefineSize * 4;
            _pixelsResize = new byte[total];
            _pixelsNew = new byte[total];

            var resizeColor = new byte[] { 0, 192, 192, 255 };
            var newColor = new byte[] { 192, 0, 0, 255 };
            for (var i = 0; i < total; i += 4)
            {
                Array.Copy(resizeColor, 0, _pixelsResize, i, 4);
                Array.Copy(newColor, 0, _pixelsNew, i, 4);
            }
        }

        private void CreateTextures(AngleGl gl)
        {
            _resizeTexture = gl.Textures.CreateTextureObject();
            _newTexture = gl.Textures.CreateTextureObject();

            gl.Textures.ActiveTexture(TextureUnit.Texture0);
            gl.Textures.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            gl.Textures.BindTexture(TextureTarget.Texture2D, _resizeTexture);
            SetTextureParameters(gl);
            DefineSquareTexture2D(gl, _resizeTexture, InitialSize, _pixelsResize ?? Array.Empty<byte>());
        }

        private void RunBenchmark(AngleGl gl)
        {
            var resizeTexture = _resizeTexture ?? throw new InvalidOperationException("Resize texture not initialized.");
            var newTexture = _newTexture ?? throw new InvalidOperationException("New texture not initialized.");
            var resizePixels = _pixelsResize ?? throw new InvalidOperationException("Pixel data not initialized.");
            var newPixels = _pixelsNew ?? throw new InvalidOperationException("Pixel data not initialized.");

            Console.WriteLine("tex_redef_microbench: 热身 {0} 帧后开始计时。", WarmupFrames);

            gl.Textures.BindTexture(TextureTarget.Texture2D, resizeTexture);
            var originalDraw = TimeDraw(gl);

            gl.State.Clear(ClearBufferMask.ColorBufferBit);
            var redefine = TimeAction(() => DefineSquareTexture2D(gl, resizeTexture, RedefineSize, resizePixels));
            var resizeDraw = TimeDraw(gl);

            gl.State.Clear(ClearBufferMask.ColorBufferBit);
            gl.Textures.BindTexture(TextureTarget.Texture2D, newTexture);
            SetTextureParameters(gl);
            var newDefine = TimeAction(() => DefineSquareTexture2D(gl, newTexture, RedefineSize, newPixels));
            var newDraw = TimeDraw(gl);

            Console.WriteLine("Original texture draw: {0:F3} ms", originalDraw);
            Console.WriteLine("Resized texture definition: {0:F3} ms", redefine);
            Console.WriteLine("Resized texture draw: {0:F3} ms", resizeDraw);
            Console.WriteLine("New texture definition: {0:F3} ms", newDefine);
            Console.WriteLine("New texture draw: {0:F3} ms", newDraw);
        }

        private double TimeDraw(AngleGl gl)
        {
            _stopwatch.Restart();
            DrawQuad(gl);
            _stopwatch.Stop();
            return _stopwatch.Elapsed.TotalMilliseconds;
        }

        private double TimeAction(Action action)
        {
            _stopwatch.Restart();
            action();
            _stopwatch.Stop();
            return _stopwatch.Elapsed.TotalMilliseconds;
        }

        private void DrawQuad(AngleGl gl)
        {
            gl.Draw.DrawElements(PrimitiveType.Triangles, s_indices.Length, IndexElementType.UnsignedShort, 0);
        }

        private void DefineSquareTexture2D(AngleGl gl, AngleTexture texture, int baseDimension, byte[] pixels)
        {
            gl.Textures.BindTexture(TextureTarget.Texture2D, texture);
            var dim = baseDimension;
            var level = 0;
            while (dim >= 1)
            {
                gl.Textures.TexImage2D(
                    TextureTarget.Texture2D,
                    level,
                    TextureInternalFormat.Rgba,
                    dim,
                    dim,
                    border: 0,
                    PixelFormat.Rgba,
                    PixelType.UnsignedByte,
                    pixels);
                dim /= 2;
                level++;
            }
        }

        private void SetTextureParameters(AngleGl gl)
        {
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        }

        private void BindGeometry(AngleGl gl)
        {
            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer ?? throw new InvalidOperationException("Vertex buffer not initialized."));
            gl.Buffers.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer ?? throw new InvalidOperationException("Index buffer not initialized."));

            gl.VertexArrays.EnableVertexAttribArray(_positionLoc);
            gl.VertexArrays.EnableVertexAttribArray(_texCoordLoc);
            gl.VertexArrays.VertexAttribPointer(_positionLoc, 3, VertexAttribType.Float, false, 5 * sizeof(float), 0);
            gl.VertexArrays.VertexAttribPointer(_texCoordLoc, 2, VertexAttribType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        }

        private void UpdateProgramBindings(AngleGl gl, AngleProgram program)
        {
            _positionLoc = gl.Programs.GetAttribLocation(program, "a_position");
            _texCoordLoc = gl.Programs.GetAttribLocation(program, "a_texCoord");
            _samplerLoc = gl.Programs.GetUniformLocation(program, "s_texture");

            if (_positionLoc < 0 || _texCoordLoc < 0 || _samplerLoc < 0)
                throw new InvalidOperationException("Failed to resolve shader bindings.");
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
                Path.Combine("Shaders", "TexRedefMicrobench.vert.glsl"),
                Path.Combine("Shaders", "TexRedefMicrobench.frag.glsl"),
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
