using AngleBridge.Sdk.HotReload;
using AngleBridge.Sdk.Interop;
using AngleBridge.Sdk.Runtime;
using AngleBridge.Utilities;
using System;
using System.IO;

namespace Demo9_MultiTexture.Plugin
{
    public sealed class MultiTextureRenderer : IRenderer, IRendererWithHostContext
    {
        private AngleGl? _gl;
        private AngleBuffer? _vertexBuffer;
        private AngleBuffer? _indexBuffer;
        private AngleVertexArray? _vao;
        private AngleTexture? _baseTexture;
        private AngleTexture? _lightTexture;
        private int _positionLoc;
        private int _texCoordLoc;
        private int _baseSamplerLoc;
        private int _lightSamplerLoc;
        private bool _useVao;
        private bool _initialized;
        private ShaderProgramReloader? _programReloader;
        private Exception? _lastReloadError;

        private readonly TextureOptions _options = TextureOptions.FromCommandLine();

        private static readonly float[] s_vertices =
        {
            // posX, posY, posZ,   u,  v
            -0.5f,  0.5f, 0.0f,   0.0f, 0.0f,
            -0.5f, -0.5f, 0.0f,   0.0f, 1.0f,
             0.5f, -0.5f, 0.0f,   1.0f, 1.0f,
             0.5f,  0.5f, 0.0f,   1.0f, 0.0f,
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

            gl.Viewport(0, 0, width, height);
            gl.Clear(ClearBufferMask.ColorBufferBit);
            gl.UseProgram(programReloader.Current);

            if (_useVao)
            {
                gl.BindVertexArray(_vao ?? throw new InvalidOperationException("Vertex array not initialized."));
            }
            else
            {
                BindGeometry(gl);
            }

            gl.ActiveTexture(TextureUnit.Texture0);
            gl.BindTexture(TextureTarget.Texture2D, _baseTexture ?? throw new InvalidOperationException("Base texture not initialized."));
            gl.Uniform1i(_baseSamplerLoc, 0);

            gl.ActiveTexture(TextureUnit.Texture1);
            gl.BindTexture(TextureTarget.Texture2D, _lightTexture ?? throw new InvalidOperationException("Light texture not initialized."));
            gl.Uniform1i(_lightSamplerLoc, 1);

            gl.DrawElements(PrimitiveType.Triangles, s_indices.Length, IndexElementType.UnsignedShort, 0);
        }

        public void Dispose()
        {
            _baseTexture?.Dispose();
            _lightTexture?.Dispose();
            _vao?.Dispose();
            _indexBuffer?.Dispose();
            _vertexBuffer?.Dispose();
            _programReloader?.Dispose();

            _baseTexture = null;
            _lightTexture = null;
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

            _baseTexture = gl.CreateTextureObject();
            _lightTexture = gl.CreateTextureObject();

            LoadTexture(gl, _baseTexture, _options.BasePath, isLight: false);
            LoadTexture(gl, _lightTexture, _options.LightPath, isLight: true);

            gl.ActiveTexture(TextureUnit.Texture0);
            gl.BindTexture(TextureTarget.Texture2D, _baseTexture);
            ApplyTexParams(gl);
            gl.ActiveTexture(TextureUnit.Texture1);
            gl.BindTexture(TextureTarget.Texture2D, _lightTexture);
            ApplyTexParams(gl);

            gl.ClearColor(0f, 0f, 0f, 0f);
            _initialized = true;
        }

        private void LoadTexture(AngleGl gl, AngleTexture texture, string? path, bool isLight)
        {
            gl.ActiveTexture(isLight ? TextureUnit.Texture1 : TextureUnit.Texture0);
            gl.BindTexture(TextureTarget.Texture2D, texture);
            gl.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            if (!string.IsNullOrWhiteSpace(path))
            {
                var fullPath = Path.GetFullPath(path);
                if (File.Exists(fullPath) && fullPath.EndsWith(".tga", StringComparison.OrdinalIgnoreCase) && TgaImage.TryLoad(fullPath, out var image) && image != null)
                {
                    gl.TexImage2D(TextureTarget.Texture2D, 0, TextureInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Rgba);
                    Console.WriteLine((isLight ? "light" : "base") + " texture loaded: " + fullPath);
                    return;
                }
                Console.Error.WriteLine("纹理加载失败（需 TGA）: " + fullPath + "，使用内置纹理。");
            }

            var pixels = isLight ? BuiltInLightTexture() : BuiltInBaseTexture();
            gl.TexImage2D(TextureTarget.Texture2D, 0, TextureInternalFormat.Rgba, pixels.width, pixels.height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels.data);
        }

        private void ApplyTexParams(AngleGl gl)
        {
            gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)_options.MinFilter);
            gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)_options.MagFilter);
            gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)_options.WrapS);
            gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)_options.WrapT);
            if (_options.GenerateMipmap)
                gl.GenerateMipmap(TextureTarget.Texture2D);
        }

        private void BindGeometry(AngleGl gl)
        {
            gl.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer ?? throw new InvalidOperationException("Vertex buffer not initialized."));
            gl.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer ?? throw new InvalidOperationException("Index buffer not initialized."));
            gl.EnableVertexAttribArray(_positionLoc);
            gl.EnableVertexAttribArray(_texCoordLoc);
            gl.VertexAttribPointer(_positionLoc, 3, VertexAttribType.Float, normalized: false, stride: 5 * sizeof(float), offsetBytes: 0);
            gl.VertexAttribPointer(_texCoordLoc, 2, VertexAttribType.Float, normalized: false, stride: 5 * sizeof(float), offsetBytes: 3 * sizeof(float));
        }

        private void UpdateProgramBindings(AngleGl gl, AngleProgram program)
        {
            _positionLoc = gl.GetAttribLocation(program, "a_position");
            _texCoordLoc = gl.GetAttribLocation(program, "a_texCoord");
            _baseSamplerLoc = gl.GetUniformLocation(program, "s_baseMap");
            _lightSamplerLoc = gl.GetUniformLocation(program, "s_lightMap");
            if (_positionLoc < 0 || _texCoordLoc < 0 || _baseSamplerLoc < 0 || _lightSamplerLoc < 0)
                throw new InvalidOperationException("Failed to resolve shader bindings.");

            if (_vao != null)
                BindGeometry(gl);
        }

        public void SetHostContext(RendererHostContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var roots = BuildShaderRoots(context, "Demo9_MultiTexture.Plugin.csproj");
            _programReloader = new ShaderProgramReloader(
                Path.Combine("Shaders", "MultiTexture.vert.glsl"),
                Path.Combine("Shaders", "MultiTexture.frag.glsl"),
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
                Path.Combine("Shaders", "MultiTexture.vert.glsl"),
                Path.Combine("Shaders", "MultiTexture.frag.glsl"),
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

        private static (int width, int height, byte[] data) BuiltInBaseTexture()
        {
            // 简单 2x2 RGBA：红、绿、蓝、白
            return (2, 2, new byte[]
            {
                255, 0,   0,   255,
                0,   255, 0,   255,
                0,   0,   255, 255,
                255, 255, 255,255,
            });
        }

        private static (int width, int height, byte[] data) BuiltInLightTexture()
        {
            // 2x2 RGBA：浅灰+亮度渐变，用于乘法光照
            return (2, 2, new byte[]
            {
                192, 192, 192, 255,
                128, 128, 128, 255,
                160, 160, 160, 255,
                224, 224, 224, 255,
            });
        }

        private sealed class TextureOptions
        {
            public string? BasePath { get; init; }
            public string? LightPath { get; init; }
            public TextureMinFilter MinFilter { get; init; } = TextureMinFilter.Linear;
            public TextureMagFilter MagFilter { get; init; } = TextureMagFilter.Linear;
            public TextureWrapMode WrapS { get; init; } = TextureWrapMode.Repeat;
            public TextureWrapMode WrapT { get; init; } = TextureWrapMode.Repeat;
            public bool GenerateMipmap { get; init; } = true;

            public static TextureOptions FromCommandLine()
            {
                var args = Environment.GetCommandLineArgs();
                string? basePath = null;
                string? lightPath = null;
                TextureMinFilter? min = null;
                TextureMagFilter? mag = null;
                TextureWrapMode? wrapS = null;
                TextureWrapMode? wrapT = null;
                bool? genMipmap = null;

                foreach (var arg in args)
                {
                    if (arg.StartsWith("--tex0=", StringComparison.OrdinalIgnoreCase) ||
                        arg.StartsWith("--tex-base=", StringComparison.OrdinalIgnoreCase))
                    {
                        var val = arg.Contains("--tex0=") ? arg.Substring("--tex0=".Length) : arg.Substring("--tex-base=".Length);
                        basePath = val;
                        continue;
                    }
                    if (arg.StartsWith("--tex1=", StringComparison.OrdinalIgnoreCase) ||
                        arg.StartsWith("--tex-light=", StringComparison.OrdinalIgnoreCase))
                    {
                        var val = arg.Contains("--tex1=") ? arg.Substring("--tex1=".Length) : arg.Substring("--tex-light=".Length);
                        lightPath = val;
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
                }

                return new TextureOptions
                {
                    BasePath = basePath,
                    LightPath = lightPath,
                    MinFilter = min ?? TextureMinFilter.Linear,
                    MagFilter = mag ?? TextureMagFilter.Linear,
                    WrapS = wrapS ?? TextureWrapMode.Repeat,
                    WrapT = wrapT ?? TextureWrapMode.Repeat,
                    GenerateMipmap = genMipmap ?? true,
                };
            }

            private static TextureMinFilter ParseMinFilter(string? value)
            {
                if (string.IsNullOrWhiteSpace(value)) return TextureMinFilter.Linear;
                var v = value.Trim().ToLowerInvariant();
                return v switch
                {
                    "nearest" => TextureMinFilter.Nearest,
                    "linear" => TextureMinFilter.Linear,
                    "nearest_mipmap_nearest" => TextureMinFilter.NearestMipmapNearest,
                    "linear_mipmap_linear" => TextureMinFilter.LinearMipmapLinear,
                    _ => TextureMinFilter.Linear,
                };
            }

            private static TextureMagFilter ParseMagFilter(string? value)
            {
                if (string.IsNullOrWhiteSpace(value)) return TextureMagFilter.Linear;
                var v = value.Trim().ToLowerInvariant();
                return v switch
                {
                    "nearest" => TextureMagFilter.Nearest,
                    "linear" => TextureMagFilter.Linear,
                    _ => TextureMagFilter.Linear,
                };
            }

            private static TextureWrapMode ParseWrap(string? value)
            {
                if (string.IsNullOrWhiteSpace(value)) return TextureWrapMode.Repeat;
                var v = value.Trim().ToLowerInvariant();
                return v switch
                {
                    "repeat" => TextureWrapMode.Repeat,
                    "clamp" or "clamp_to_edge" => TextureWrapMode.ClampToEdge,
                    "mirror" or "mirrored_repeat" => TextureWrapMode.MirroredRepeat,
                    _ => TextureWrapMode.Repeat,
                };
            }
        }
    }
}
