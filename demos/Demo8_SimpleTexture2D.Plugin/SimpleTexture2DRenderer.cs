using AngleBridge.Sdk.HotReload;
using AngleBridge.Sdk.Interop;
using AngleBridge.Sdk.Runtime;
using AngleBridge.Utilities;
using System;
using System.IO;

namespace Demo8_SimpleTexture2D.Plugin
{
    public sealed class SimpleTexture2DRenderer : IRenderer, IRendererWithHostContext
    {
        private AngleGl? _gl;
        private AngleBuffer? _vertexBuffer;
        private AngleBuffer? _indexBuffer;
        private AngleVertexArray? _vao;
        private AngleTexture? _texture;
        private int _positionLoc;
        private int _texCoordLoc;
        private int _samplerLoc;
        private bool _useVao;
        private bool _initialized;
        private bool _expandWrapUv;
        private ShaderProgramReloader? _programReloader;
        private Exception? _lastReloadError;

        private readonly TextureOptions _options = TextureOptions.FromCommandLine();

        private static readonly float[] s_verticesNormal =
        {
            // posX, posY, posZ,   u,  v
            -0.5f,  0.5f, 0.0f,   0.0f, 0.0f,
            -0.5f, -0.5f, 0.0f,   0.0f, 1.0f,
             0.5f, -0.5f, 0.0f,   1.0f, 1.0f,
             0.5f,  0.5f, 0.0f,   1.0f, 0.0f,
        };

        private static readonly float[] s_verticesWrap =
        {
            // posX, posY, posZ,        u,    v   (UV 扩大到 [-0.5, 1.5]，便于观察 wrap)
            -0.5f,  0.5f, 0.0f,   -0.5f, -0.5f,
            -0.5f, -0.5f, 0.0f,   -0.5f,  1.5f,
             0.5f, -0.5f, 0.0f,    1.5f,  1.5f,
             0.5f,  0.5f, 0.0f,    1.5f, -0.5f,
        };

        private static readonly ushort[] s_indices = { 0, 1, 2, 0, 2, 3 };

        // 2x2 RGB 与 ANGLE 原版保持一致：红、绿、蓝、黄
        private static readonly byte[] s_textureData =
        {
            255, 0,   0,
            0,   255, 0,
            0,   0,   255,
            255, 255, 0,
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

            gl.State.Viewport(0, 0, width, height);
            gl.State.Clear(ClearBufferMask.ColorBufferBit);
            gl.Programs.UseProgram(programReloader.Current);

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

            gl.Draw.DrawElements(PrimitiveType.Triangles, s_indices.Length, IndexElementType.UnsignedShort, 0);
        }

        public void Dispose()
        {
            if (_texture != null) _texture.Dispose();
            if (_vao != null) _vao.Dispose();
            if (_indexBuffer != null) _indexBuffer.Dispose();
            if (_vertexBuffer != null) _vertexBuffer.Dispose();
            if (_programReloader != null) _programReloader.Dispose();

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
            var vertices = _options.ExpandWrapUv ? s_verticesWrap : s_verticesNormal;
            _expandWrapUv = _options.ExpandWrapUv;
            gl.Buffers.BufferData(BufferTarget.ArrayBuffer, vertices, BufferUsage.StaticDraw);

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

            if (!TryLoadExternalTexture(gl))
            {
                gl.Textures.TexImage2D(
                    TextureTarget.Texture2D,
                    level: 0,
                    internalFormat: TextureInternalFormat.Rgb,
                    width: 2,
                    height: 2,
                    border: 0,
                    format: PixelFormat.Rgb,
                    type: PixelType.UnsignedByte,
                    pixels: s_textureData);
                Console.WriteLine("SIMPLE_TEX_PATH 未设置或加载失败，使用内置 2x2 RGB 纹理。");
            }

            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)_options.MinFilter);
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)_options.MagFilter);
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)_options.WrapS);
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)_options.WrapT);
            if (_options.GenerateMipmap)
                gl.Textures.GenerateMipmap(TextureTarget.Texture2D);

            gl.State.ClearColor(0f, 0f, 0f, 0f);
            _initialized = true;
        }

        private bool TryLoadExternalTexture(AngleGl gl)
        {
            var path = _options.Path;
            if (string.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                var fullPath = Path.GetFullPath(path);
                if (!File.Exists(fullPath))
                {
                Console.Error.WriteLine("SIMPLE_TEX_PATH 指向的文件不存在: " + fullPath);
                return false;
            }

            if (!fullPath.EndsWith(".tga", StringComparison.OrdinalIgnoreCase))
            {
                Console.Error.WriteLine("仅支持 TGA 文件： " + fullPath);
                return false;
            }

            if (!TgaImage.TryLoad(fullPath, out var image) || image == null)
            {
                Console.Error.WriteLine("TGA 加载失败: " + fullPath);
                return false;
            }

            gl.Textures.TexImage2D(
                TextureTarget.Texture2D,
                    level: 0,
                    internalFormat: TextureInternalFormat.Rgba,
                    width: image.Width,
                    height: image.Height,
                    border: 0,
                format: PixelFormat.Rgba,
                type: PixelType.UnsignedByte,
                pixels: image.Rgba);
                Console.WriteLine("已加载外部纹理: " + fullPath);
                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("加载外部纹理发生异常: " + ex.Message);
                return false;
            }
        }

        private void BindGeometry(AngleGl gl)
        {
            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer ?? throw new InvalidOperationException("Vertex buffer not initialized."));
            gl.Buffers.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer ?? throw new InvalidOperationException("Index buffer not initialized."));
            gl.VertexArrays.EnableVertexAttribArray(_positionLoc);
            gl.VertexArrays.EnableVertexAttribArray(_texCoordLoc);
            gl.VertexArrays.VertexAttribPointer(_positionLoc, 3, VertexAttribType.Float, normalized: false, stride: 5 * sizeof(float), offsetBytes: 0);
            gl.VertexArrays.VertexAttribPointer(_texCoordLoc, 2, VertexAttribType.Float, normalized: false, stride: 5 * sizeof(float), offsetBytes: 3 * sizeof(float));
        }

        private void UpdateProgramBindings(AngleGl gl, AngleProgram program)
        {
            _positionLoc = gl.Programs.GetAttribLocation(program, "a_position");
            _texCoordLoc = gl.Programs.GetAttribLocation(program, "a_texCoord");
            _samplerLoc = gl.Programs.GetUniformLocation(program, "s_texture");
            if (_positionLoc < 0 || _texCoordLoc < 0 || _samplerLoc < 0)
                throw new InvalidOperationException("Failed to resolve shader bindings.");

            if (_vao != null)
                BindGeometry(gl);
        }

        public void SetHostContext(RendererHostContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var roots = BuildShaderRoots(context, "Demo8_SimpleTexture2D.Plugin.csproj");
            _programReloader = new ShaderProgramReloader(
                Path.Combine("Shaders", "SimpleTexture2D.vert.glsl"),
                Path.Combine("Shaders", "SimpleTexture2D.frag.glsl"),
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
                Path.Combine("Shaders", "SimpleTexture2D.vert.glsl"),
                Path.Combine("Shaders", "SimpleTexture2D.frag.glsl"),
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

        private sealed class TextureOptions
        {
            public string? Path { get; init; }
            public TextureMinFilter MinFilter { get; init; } = TextureMinFilter.Nearest;
            public TextureMagFilter MagFilter { get; init; } = TextureMagFilter.Nearest;
            public TextureWrapMode WrapS { get; init; } = TextureWrapMode.Repeat;
            public TextureWrapMode WrapT { get; init; } = TextureWrapMode.Repeat;
            public bool GenerateMipmap { get; init; }
            public bool ExpandWrapUv { get; init; }

            public static TextureOptions FromCommandLine()
            {
                var args = Environment.GetCommandLineArgs();
                string? path = null;
                TextureMinFilter? min = null;
                TextureMagFilter? mag = null;
                TextureWrapMode? wrapS = null;
                TextureWrapMode? wrapT = null;
                bool? genMipmap = null;
                bool expandWrapUv = false;

                foreach (var arg in args)
                {
                    if (arg.StartsWith("--tex=", StringComparison.OrdinalIgnoreCase))
                    {
                        path = arg.Substring("--tex=".Length);
                        continue;
                    }
                    if (arg.StartsWith("--tex-wrap-preset=", StringComparison.OrdinalIgnoreCase))
                    {
                        var preset = arg.Substring("--tex-wrap-preset=".Length).Trim().ToLowerInvariant();
                        switch (preset)
                        {
                            case "repeat":
                                wrapS = TextureWrapMode.Repeat;
                                wrapT = TextureWrapMode.Repeat;
                                break;
                            case "clamp":
                            case "clamp_to_edge":
                                wrapS = TextureWrapMode.ClampToEdge;
                                wrapT = TextureWrapMode.ClampToEdge;
                                break;
                            case "mirror":
                            case "mirrored_repeat":
                                wrapS = TextureWrapMode.MirroredRepeat;
                                wrapT = TextureWrapMode.MirroredRepeat;
                                break;
                        }
                        continue;
                    }
                    if (arg.StartsWith("--tex-min=", StringComparison.OrdinalIgnoreCase))
                    {
                        min = ParseMinFilter(arg.Substring("--tex-min=".Length));
                        continue;
                    }
                    if (arg.StartsWith("--tex-mag=", StringComparison.OrdinalIgnoreCase))
                    {
                        mag = ParseMagFilter(arg.Substring("--tex-mag=".Length));
                        continue;
                    }
                    if (arg.StartsWith("--tex-wrap-s=", StringComparison.OrdinalIgnoreCase))
                    {
                        wrapS = ParseWrap(arg.Substring("--tex-wrap-s=".Length));
                        continue;
                    }
                    if (arg.StartsWith("--tex-wrap-t=", StringComparison.OrdinalIgnoreCase))
                    {
                        wrapT = ParseWrap(arg.Substring("--tex-wrap-t=".Length));
                        continue;
                    }
                    if (arg.Equals("--tex-mipmap", StringComparison.OrdinalIgnoreCase))
                    {
                        genMipmap = true;
                        continue;
                    }
                    if (arg.Equals("--tex-no-mipmap", StringComparison.OrdinalIgnoreCase))
                    {
                        genMipmap = false;
                        continue;
                    }
                    if (arg.Equals("--tex-wrap-uv", StringComparison.OrdinalIgnoreCase))
                    {
                        expandWrapUv = true;
                        continue;
                    }
                }

                return new TextureOptions
                {
                    Path = path,
                    MinFilter = min ?? TextureMinFilter.Nearest,
                    MagFilter = mag ?? TextureMagFilter.Nearest,
                    WrapS = wrapS ?? TextureWrapMode.Repeat,
                    WrapT = wrapT ?? TextureWrapMode.Repeat,
                    GenerateMipmap = genMipmap ?? false,
                    ExpandWrapUv = expandWrapUv,
                };
            }

            private static TextureMinFilter ParseMinFilter(string? value)
            {
                if (string.IsNullOrWhiteSpace(value)) return TextureMinFilter.Nearest;
                var v = value.Trim().ToLowerInvariant();
                return v switch
                {
                    "linear" => TextureMinFilter.Linear,
                    "nearest_mipmap_nearest" => TextureMinFilter.NearestMipmapNearest,
                    "linear_mipmap_linear" => TextureMinFilter.LinearMipmapLinear,
                    _ => TextureMinFilter.Nearest,
                };
            }

            private static TextureMagFilter ParseMagFilter(string? value)
            {
                if (string.IsNullOrWhiteSpace(value)) return TextureMagFilter.Nearest;
                var v = value.Trim().ToLowerInvariant();
                return v switch
                {
                    "linear" => TextureMagFilter.Linear,
                    _ => TextureMagFilter.Nearest,
                };
            }

            private static TextureWrapMode ParseWrap(string? value)
            {
                if (string.IsNullOrWhiteSpace(value)) return TextureWrapMode.Repeat;
                var v = value.Trim().ToLowerInvariant();
                return v switch
                {
                    "clamp" or "clamp_to_edge" => TextureWrapMode.ClampToEdge,
                    "mirrored_repeat" => TextureWrapMode.MirroredRepeat,
                    _ => TextureWrapMode.Repeat,
                };
            }

            private static bool ParseBool(string? value)
            {
                if (string.IsNullOrWhiteSpace(value)) return false;
                return value.Trim().Equals("true", StringComparison.OrdinalIgnoreCase) ||
                       value.Trim().Equals("1", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
