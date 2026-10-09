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
            gl.Textures.BindTexture(TextureTarget.Texture2D, _baseTexture ?? throw new InvalidOperationException("Base texture not initialized."));
            gl.Programs.Uniform1i(_baseSamplerLoc, 0);

            gl.Textures.ActiveTexture(TextureUnit.Texture1);
            gl.Textures.BindTexture(TextureTarget.Texture2D, _lightTexture ?? throw new InvalidOperationException("Light texture not initialized."));
            gl.Programs.Uniform1i(_lightSamplerLoc, 1);

            gl.Draw.DrawElements(PrimitiveType.Triangles, s_indices.Length, IndexElementType.UnsignedShort, 0);
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

            _baseTexture = gl.Textures.CreateTextureObject();
            _lightTexture = gl.Textures.CreateTextureObject();

            LoadTexture(gl, _baseTexture, _options.BasePath, isLight: false);
            LoadTexture(gl, _lightTexture, _options.LightPath, isLight: true);

            gl.Textures.ActiveTexture(TextureUnit.Texture0);
            gl.Textures.BindTexture(TextureTarget.Texture2D, _baseTexture);
            ApplyTexParams(gl);
            gl.Textures.ActiveTexture(TextureUnit.Texture1);
            gl.Textures.BindTexture(TextureTarget.Texture2D, _lightTexture);
            ApplyTexParams(gl);

            gl.State.ClearColor(0f, 0f, 0f, 0f);
            _initialized = true;
        }

        private void LoadTexture(AngleGl gl, AngleTexture texture, string? path, bool isLight)
        {
            gl.Textures.ActiveTexture(isLight ? TextureUnit.Texture1 : TextureUnit.Texture0);
            gl.Textures.BindTexture(TextureTarget.Texture2D, texture);
            gl.Textures.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            if (!string.IsNullOrWhiteSpace(path))
            {
                var fullPath = Path.GetFullPath(path);
                if (File.Exists(fullPath) && fullPath.EndsWith(".tga", StringComparison.OrdinalIgnoreCase) && TgaImage.TryLoad(fullPath, out var image) && image != null)
                {
                    gl.Textures.TexImage2D(TextureTarget.Texture2D, 0, TextureInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Rgba);
                    Console.WriteLine((isLight ? "light" : "base") + " texture loaded: " + fullPath);
                    return;
                }
                Console.Error.WriteLine("Texture load failed (TGA required): " + fullPath + "; using the built-in texture.");
            }

            var pixels = isLight ? BuiltInLightTexture() : BuiltInBaseTexture();
            gl.Textures.TexImage2D(TextureTarget.Texture2D, 0, TextureInternalFormat.Rgba, pixels.width, pixels.height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels.data);
        }

        private void ApplyTexParams(AngleGl gl)
        {
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)_options.MinFilter);
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)_options.MagFilter);
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)_options.WrapS);
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)_options.WrapT);
            if (_options.GenerateMipmap)
                gl.Textures.GenerateMipmap(TextureTarget.Texture2D);
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
            _baseSamplerLoc = gl.Programs.GetUniformLocation(program, "s_baseMap");
            _lightSamplerLoc = gl.Programs.GetUniformLocation(program, "s_lightMap");
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
            // Simple 2x2 RGBA texture: red, green, blue, white
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
            // 2x2 RGBA texture: light gray with a brightness gradient for multiplicative lighting
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
