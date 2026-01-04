using AngleBridge.Sdk.HotReload;
using AngleBridge.Sdk.Interop;
using AngleBridge.Sdk.Runtime;
using System;
using System.IO;

namespace Demo10_MipMap2D.Plugin
{
    public sealed class MipMap2DRenderer : IRenderer, IRendererWithHostContext
    {
        private AngleGl? _gl;
        private AngleBuffer? _vertexBuffer;
        private AngleBuffer? _indexBuffer;
        private AngleVertexArray? _vao;
        private AngleTexture? _texture;
        private ShaderProgramReloader? _programReloader;
        private Exception? _lastReloadError;
        private bool _initialized;
        private bool _useVao;
        private int _positionLoc;
        private int _texCoordLoc;
        private int _samplerLoc;
        private int _offsetLoc;
        private float _maxAniso = 1f;

        private static readonly float[] s_vertices =
        {
            // posX, posY, posZ, posW,  u,  v
            -0.25f,  0.5f, 0f, 5f,      0f, 0f,
            -0.25f, -0.5f, 0f, 1f,      0f, 1f,
             0.25f, -0.5f, 0f, 1f,      1f, 1f,
             0.25f,  0.5f, 0f, 5f,      1f, 0f,
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

            var reloader = _programReloader ?? throw new InvalidOperationException("Program reloader not initialized.");
            if (reloader.TryReload(gl))
                UpdateProgramBindings(gl, reloader.Current);

            var reloadError = reloader.LastError;
            if (reloadError != null && !ReferenceEquals(reloadError, _lastReloadError))
            {
                _lastReloadError = reloadError;
                Console.Error.WriteLine(reloadError);
            }

            gl.State.Viewport(0, 0, width, height);
            gl.State.Clear(ClearBufferMask.ColorBufferBit);
            gl.Programs.UseProgram(reloader.Current);

            if (_useVao)
            {
                gl.VertexArrays.BindVertexArray(_vao ?? throw new InvalidOperationException("Vertex array not initialized."));
            }
            else
            {
                BindGeometry(gl);
            }

            gl.Textures.ActiveTexture(TextureUnit.Texture0);
            gl.Textures.BindTexture(TextureTarget.Texture2D, _texture ?? throw new InvalidOperationException("Texture not initialized."));
            gl.Programs.Uniform1i(_samplerLoc, 0);

            DrawQuad(gl, TextureMinFilter.Nearest, offset: -0.6f, setAniso: false);
            DrawQuad(gl, TextureMinFilter.LinearMipmapLinear, offset: 0f, setAniso: false);
            DrawQuad(gl, TextureMinFilter.LinearMipmapLinear, offset: 0.6f, setAniso: true);
        }

        public void Dispose()
        {
            _texture?.Dispose();
            _vao?.Dispose();
            _indexBuffer?.Dispose();
            _vertexBuffer?.Dispose();
            _programReloader?.Dispose();

            _texture = null;
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

            _texture = gl.Textures.CreateTextureObject();
            gl.Textures.ActiveTexture(TextureUnit.Texture0);
            gl.Textures.BindTexture(TextureTarget.Texture2D, _texture);
            gl.Textures.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            UploadMipChain(gl);
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            if (gl.Caps.TryGetFloat(GetFloatParameter.MaxTextureMaxAnisotropyExt, out var maxAniso) && maxAniso > 0f)
                _maxAniso = maxAniso;

            gl.State.ClearColor(0f, 0f, 0f, 0f);
            _initialized = true;
        }

        private void BindGeometry(AngleGl gl)
        {
            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer ?? throw new InvalidOperationException("Vertex buffer not initialized."));
            gl.Buffers.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer ?? throw new InvalidOperationException("Index buffer not initialized."));
            gl.VertexArrays.EnableVertexAttribArray(_positionLoc);
            gl.VertexArrays.EnableVertexAttribArray(_texCoordLoc);
            gl.VertexArrays.VertexAttribPointer(_positionLoc, 4, VertexAttribType.Float, false, 6 * sizeof(float), 0);
            gl.VertexArrays.VertexAttribPointer(_texCoordLoc, 2, VertexAttribType.Float, false, 6 * sizeof(float), 4 * sizeof(float));
        }

        private void UpdateProgramBindings(AngleGl gl, AngleProgram program)
        {
            _positionLoc = gl.Programs.GetAttribLocation(program, "a_position");
            _texCoordLoc = gl.Programs.GetAttribLocation(program, "a_texCoord");
            _samplerLoc = gl.Programs.GetUniformLocation(program, "s_texture");
            _offsetLoc = gl.Programs.GetUniformLocation(program, "u_offset");
            if (_positionLoc < 0 || _texCoordLoc < 0 || _samplerLoc < 0 || _offsetLoc < 0)
                throw new InvalidOperationException("Failed to resolve shader bindings.");

            if (_vao != null)
                BindGeometry(gl);
        }

        public void SetHostContext(RendererHostContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var roots = new[]
            {
                context.PluginDirectory,
                AppContext.BaseDirectory,
                Directory.GetCurrentDirectory(),
            };
            _programReloader = new ShaderProgramReloader(
                Path.Combine("Shaders", "MipMap2D.vert.glsl"),
                Path.Combine("Shaders", "MipMap2D.frag.glsl"),
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
                Path.Combine("Shaders", "MipMap2D.vert.glsl"),
                Path.Combine("Shaders", "MipMap2D.frag.glsl"),
                roots);
        }

        private void DrawQuad(AngleGl gl, TextureMinFilter minFilter, float offset, bool setAniso)
        {
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            if (setAniso && _maxAniso > 1f)
            {
                gl.Textures.TexParameterf(TextureTarget.Texture2D, TextureParameterName.TextureMaxAnisotropyExt, _maxAniso);
            }
            else if (setAniso)
            {
                gl.Textures.TexParameterf(TextureTarget.Texture2D, TextureParameterName.TextureMaxAnisotropyExt, 1f);
            }

            gl.Programs.Uniform4f(_offsetLoc, offset, 0f, 0f, 0f);
            gl.Draw.DrawElements(PrimitiveType.Triangles, s_indices.Length, IndexElementType.UnsignedShort, 0);

            if (setAniso && _maxAniso > 1f)
                gl.Textures.TexParameterf(TextureTarget.Texture2D, TextureParameterName.TextureMaxAnisotropyExt, 1f);
        }

        private void UploadMipChain(AngleGl gl)
        {
            const int width = 256;
            const int height = 256;
            const int checkerSize = 8;

            var pixels = new byte[width * height * 3];
            var i = 0;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    byte r = 0;
                    byte b = 0;
                    var yBit = (y / checkerSize) & 1;
                    if (((x / checkerSize) & 1) == 0)
                    {
                        r = (byte)(255 * yBit);
                        b = (byte)(255 * (1 - yBit));
                    }
                    else
                    {
                        b = (byte)(255 * yBit);
                        r = (byte)(255 * (1 - yBit));
                    }

                    pixels[i++] = r;
                    pixels[i++] = 0;
                    pixels[i++] = b;
                }
            }

            gl.Textures.TexImage2D(TextureTarget.Texture2D, 0, TextureInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, pixels);
            gl.Textures.GenerateMipmap(TextureTarget.Texture2D);
        }

    }
}
