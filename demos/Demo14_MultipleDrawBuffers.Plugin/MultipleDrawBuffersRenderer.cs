using AngleBridge.Sdk.HotReload;
using AngleBridge.Sdk.Interop;
using AngleBridge.Sdk.Runtime;
using System;
using System.IO;

namespace Demo14_MultipleDrawBuffers.Plugin
{
    public sealed class MultipleDrawBuffersRenderer : IRenderer, IRendererWithHostContext
    {
        private const int AttachmentCount = 4;

        private AngleGl? _gl;
        private AngleBuffer? _vertexBuffer;
        private AngleBuffer? _indexBuffer;
        private AngleVertexArray? _vao;
        private AngleTexture? _sourceTexture;
        private AngleTexture[]? _rtTextures;
        private AngleFramebuffer? _framebuffer;
        private ShaderProgramReloader? _mrtProgramReloader;
        private ShaderProgramReloader? _copyProgramReloader;
        private Exception? _lastMrtError;
        private Exception? _lastCopyError;
        private int _mrtPositionLoc;
        private int _mrtTexCoordLoc;
        private int _mrtSamplerLoc;
        private int _copyPositionLoc;
        private int _copyTexCoordLoc;
        private int _copySamplerLoc;
        private bool _useVao;
        private bool _initialized;
        private int _fbWidth;
        private int _fbHeight;

        private static readonly float[] s_vertices =
        {
            // posX, posY, posZ,   u,  v
            -0.8f,  0.8f, 0.0f,   0.0f, 0.0f,
            -0.8f, -0.8f, 0.0f,   0.0f, 1.0f,
             0.8f, -0.8f, 0.0f,   1.0f, 1.0f,
             0.8f,  0.8f, 0.0f,   1.0f, 0.0f,
        };

        private static readonly ushort[] s_indices = { 0, 1, 2, 0, 2, 3 };

        private static readonly byte[] s_baseTexture =
        {
            255,   0,   0, 255,
              0, 255,   0, 255,
              0,   0, 255, 255,
            255, 255,   0, 255,
        };

        private static readonly DrawBuffer[] s_drawBuffers =
        {
            DrawBuffer.ColorAttachment0,
            DrawBuffer.ColorAttachment1,
            DrawBuffer.ColorAttachment2,
            DrawBuffer.ColorAttachment3,
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

            ReloadPrograms(gl);
            EnsureFramebuffer(gl, width, height);

            gl.Framebuffers.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _framebuffer ?? throw new InvalidOperationException("Framebuffer not initialized."));
            gl.Framebuffers.DrawBuffers(s_drawBuffers);

            gl.State.Viewport(0, 0, width, height);
            gl.State.Clear(ClearBufferMask.ColorBufferBit);
            gl.Programs.UseProgram(_mrtProgramReloader?.Current ?? throw new InvalidOperationException("MRT program not loaded."));
            BindGeometry(gl);

            gl.Textures.ActiveTexture(TextureUnit.Texture0);
            gl.Textures.BindTexture(TextureTarget.Texture2D, _sourceTexture ?? throw new InvalidOperationException("Source texture not initialized."));
            gl.Programs.Uniform1i(_mrtSamplerLoc, 0);
            gl.Draw.DrawElements(PrimitiveType.Triangles, s_indices.Length, IndexElementType.UnsignedShort, 0);

            gl.Framebuffers.BindFramebuffer(FramebufferTarget.Framebuffer, new GlFramebufferHandle(0));
            gl.State.Clear(ClearBufferMask.ColorBufferBit);
            gl.Programs.UseProgram(_copyProgramReloader?.Current ?? throw new InvalidOperationException("Copy program not loaded."));
            BindGeometry(gl);
            gl.Textures.ActiveTexture(TextureUnit.Texture0);

            var halfW = width / 2;
            var halfH = height / 2;
            for (var i = 0; i < AttachmentCount; i++)
            {
                var tex = _rtTextures?[i] ?? throw new InvalidOperationException("RT texture not initialized.");
                gl.Textures.BindTexture(TextureTarget.Texture2D, tex);
                gl.Programs.Uniform1i(_copySamplerLoc, 0);

                switch (i)
                {
                    case 0:
                        gl.State.Viewport(0, 0, halfW, halfH);
                        break;
                    case 1:
                        gl.State.Viewport(halfW, 0, halfW, halfH);
                        break;
                    case 2:
                        gl.State.Viewport(0, halfH, halfW, halfH);
                        break;
                    default:
                        gl.State.Viewport(halfW, halfH, halfW, halfH);
                        break;
                }

                gl.Draw.DrawElements(PrimitiveType.Triangles, s_indices.Length, IndexElementType.UnsignedShort, 0);
            }
        }

        public void Dispose()
        {
            DisposeAttachments();
            _framebuffer?.Dispose();
            _sourceTexture?.Dispose();
            _vao?.Dispose();
            _indexBuffer?.Dispose();
            _vertexBuffer?.Dispose();
            _mrtProgramReloader?.Dispose();
            _copyProgramReloader?.Dispose();

            _framebuffer = null;
            _sourceTexture = null;
            _vao = null;
            _indexBuffer = null;
            _vertexBuffer = null;
            _gl = null;
            _initialized = false;
            _useVao = false;
            _fbWidth = 0;
            _fbHeight = 0;
            _mrtProgramReloader = null;
            _copyProgramReloader = null;
            _lastCopyError = null;
            _lastMrtError = null;
        }

        public void SetHostContext(RendererHostContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var roots = BuildShaderRoots(context, "Demo14_MultipleDrawBuffers.Plugin.csproj");
            _mrtProgramReloader = new ShaderProgramReloader(
                Path.Combine("Shaders", "MultipleDrawBuffers.vert.glsl"),
                Path.Combine("Shaders", "MultipleDrawBuffersMrt.frag.glsl"),
                roots);
            _copyProgramReloader = new ShaderProgramReloader(
                Path.Combine("Shaders", "MultipleDrawBuffers.vert.glsl"),
                Path.Combine("Shaders", "MultipleDrawBuffersCopy.frag.glsl"),
                roots);
        }

        private void Initialize(AngleGl gl)
        {
            if (!gl.Framebuffers.IsDrawBuffersAvailable)
                throw new NotSupportedException("glDrawBuffers/glDrawBuffersEXT is unavailable; multiple_draw_buffers cannot run.");

            EnsureProgramReloaders();
            var mrtReloader = _mrtProgramReloader ?? throw new InvalidOperationException("MRT program reloader not initialized.");
            var copyReloader = _copyProgramReloader ?? throw new InvalidOperationException("Copy program reloader not initialized.");
            mrtReloader.Load(gl);
            copyReloader.Load(gl);
            UpdateProgramBindings(gl);

            _vertexBuffer = gl.Buffers.CreateBufferObject();
            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
            gl.Buffers.BufferData(BufferTarget.ArrayBuffer, s_vertices, BufferUsage.StaticDraw);

            _indexBuffer = gl.Buffers.CreateBufferObject();
            gl.Buffers.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
            gl.Buffers.BufferData(BufferTarget.ElementArrayBuffer, s_indices, BufferUsage.StaticDraw);

            try
            {
                _vao = gl.VertexArrays.CreateVertexArrayObject();
                _useVao = true;
                gl.VertexArrays.BindVertexArray(_vao);
                BindGeometry(gl);
            }
            catch (NotSupportedException)
            {
                _useVao = false;
                _vao = null;
            }

            _sourceTexture = gl.Textures.CreateTextureObject();
            gl.Textures.ActiveTexture(TextureUnit.Texture0);
            gl.Textures.BindTexture(TextureTarget.Texture2D, _sourceTexture);
            gl.Textures.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            gl.Textures.TexImage2D(
                TextureTarget.Texture2D,
                level: 0,
                internalFormat: TextureInternalFormat.Rgba,
                width: 2,
                height: 2,
                border: 0,
                format: PixelFormat.Rgba,
                type: PixelType.UnsignedByte,
                pixels: s_baseTexture);
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            gl.State.ClearColor(0f, 0f, 0f, 0f);
            _initialized = true;
        }

        private void EnsureFramebuffer(AngleGl gl, int width, int height)
        {
            if (width <= 0 || height <= 0) throw new ArgumentOutOfRangeException("Framebuffer size must be positive.");

            if (_framebuffer == null)
                _framebuffer = gl.Framebuffers.CreateFramebufferObject();

            if (_rtTextures != null && width == _fbWidth && height == _fbHeight)
                return;

            DisposeAttachments();
            _rtTextures = new AngleTexture[AttachmentCount];

            gl.Framebuffers.BindFramebuffer(FramebufferTarget.Framebuffer, _framebuffer);
            gl.Textures.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            for (var i = 0; i < AttachmentCount; i++)
            {
                var tex = gl.Textures.CreateTextureObject();
                gl.Textures.BindTexture(TextureTarget.Texture2D, tex);
                gl.Textures.TexImage2D(
                    TextureTarget.Texture2D,
                    level: 0,
                    internalFormat: TextureInternalFormat.Rgba,
                    width: width,
                    height: height,
                    border: 0,
                    format: PixelFormat.Rgba,
                    type: PixelType.UnsignedByte,
                    pixels: null);
                gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

                var attachment = (FramebufferAttachment)((int)FramebufferAttachment.ColorAttachment0 + i);
                gl.Framebuffers.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachment, TextureTarget.Texture2D, tex, 0);
                _rtTextures[i] = tex;
            }

            var status = gl.Framebuffers.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferStatus.FramebufferComplete)
                throw new InvalidOperationException("Framebuffer incomplete: " + status);

            gl.Framebuffers.BindFramebuffer(FramebufferTarget.Framebuffer, new GlFramebufferHandle(0));
            _fbWidth = width;
            _fbHeight = height;
        }

        private void ReloadPrograms(AngleGl gl)
        {
            var mrt = _mrtProgramReloader ?? throw new InvalidOperationException("MRT program reloader not initialized.");
            var copy = _copyProgramReloader ?? throw new InvalidOperationException("Copy program reloader not initialized.");

            var updated = mrt.TryReload(gl);
            updated |= copy.TryReload(gl);

            var mrtError = mrt.LastError;
            if (mrtError != null && !ReferenceEquals(mrtError, _lastMrtError))
            {
                _lastMrtError = mrtError;
                Console.Error.WriteLine(mrtError);
            }

            var copyError = copy.LastError;
            if (copyError != null && !ReferenceEquals(copyError, _lastCopyError))
            {
                _lastCopyError = copyError;
                Console.Error.WriteLine(copyError);
            }

            if (updated)
                UpdateProgramBindings(gl);
        }

        private void BindGeometry(AngleGl gl)
        {
            if (_useVao && _vao != null)
                gl.VertexArrays.BindVertexArray(_vao);

            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer ?? throw new InvalidOperationException("Vertex buffer not initialized."));
            gl.Buffers.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer ?? throw new InvalidOperationException("Index buffer not initialized."));

            gl.VertexArrays.EnableVertexAttribArray(_mrtPositionLoc);
            gl.VertexArrays.EnableVertexAttribArray(_mrtTexCoordLoc);
            gl.VertexArrays.EnableVertexAttribArray(_copyPositionLoc);
            gl.VertexArrays.EnableVertexAttribArray(_copyTexCoordLoc);

            gl.VertexArrays.VertexAttribPointer(_mrtPositionLoc, 3, VertexAttribType.Float, false, 5 * sizeof(float), 0);
            gl.VertexArrays.VertexAttribPointer(_mrtTexCoordLoc, 2, VertexAttribType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            gl.VertexArrays.VertexAttribPointer(_copyPositionLoc, 3, VertexAttribType.Float, false, 5 * sizeof(float), 0);
            gl.VertexArrays.VertexAttribPointer(_copyTexCoordLoc, 2, VertexAttribType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        }

        private void UpdateProgramBindings(AngleGl gl)
        {
            var mrtProgram = _mrtProgramReloader?.Current ?? throw new InvalidOperationException("MRT program not loaded.");
            var copyProgram = _copyProgramReloader?.Current ?? throw new InvalidOperationException("Copy program not loaded.");

            _mrtPositionLoc = gl.Programs.GetAttribLocation(mrtProgram, "a_position");
            _mrtTexCoordLoc = gl.Programs.GetAttribLocation(mrtProgram, "a_texCoord");
            _mrtSamplerLoc = gl.Programs.GetUniformLocation(mrtProgram, "s_texture");

            _copyPositionLoc = gl.Programs.GetAttribLocation(copyProgram, "a_position");
            _copyTexCoordLoc = gl.Programs.GetAttribLocation(copyProgram, "a_texCoord");
            _copySamplerLoc = gl.Programs.GetUniformLocation(copyProgram, "s_texture");

            if (_mrtPositionLoc < 0 || _mrtTexCoordLoc < 0 || _mrtSamplerLoc < 0)
                throw new InvalidOperationException("Failed to resolve MRT shader bindings.");
            if (_copyPositionLoc < 0 || _copyTexCoordLoc < 0 || _copySamplerLoc < 0)
                throw new InvalidOperationException("Failed to resolve copy shader bindings.");

            if (_useVao && _vao != null)
                BindGeometry(gl);
        }

        private void DisposeAttachments()
        {
            if (_rtTextures == null) return;
            foreach (var tex in _rtTextures)
                tex?.Dispose();
            _rtTextures = null;
        }

        private void EnsureProgramReloaders()
        {
            if (_mrtProgramReloader != null && _copyProgramReloader != null) return;
            var roots = new[]
            {
                AppContext.BaseDirectory,
                Directory.GetCurrentDirectory(),
            };
            _mrtProgramReloader ??= new ShaderProgramReloader(
                Path.Combine("Shaders", "MultipleDrawBuffers.vert.glsl"),
                Path.Combine("Shaders", "MultipleDrawBuffersMrt.frag.glsl"),
                roots);
            _copyProgramReloader ??= new ShaderProgramReloader(
                Path.Combine("Shaders", "MultipleDrawBuffers.vert.glsl"),
                Path.Combine("Shaders", "MultipleDrawBuffersCopy.frag.glsl"),
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
