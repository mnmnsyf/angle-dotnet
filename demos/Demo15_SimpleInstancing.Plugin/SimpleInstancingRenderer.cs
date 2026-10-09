using AngleBridge.Sdk.HotReload;
using AngleBridge.Sdk.Interop;
using AngleBridge.Sdk.Runtime;
using System;
using System.Collections.Generic;
using System.IO;

namespace Demo15_SimpleInstancing.Plugin
{
    public sealed class SimpleInstancingRenderer : IRenderer, IRendererWithHostContext
    {
        private const float QuadRadius = 0.01f;

        private AngleGl? _gl;
        private AngleBuffer? _vertexBuffer;
        private AngleBuffer? _indexBuffer;
        private AngleBuffer? _instanceBuffer;
        private AngleVertexArray? _vao;
        private AngleTexture? _texture;
        private ShaderProgramReloader? _programReloader;
        private Exception? _lastReloadError;
        private int _positionLoc;
        private int _texCoordLoc;
        private int _instanceLoc;
        private int _samplerLoc;
        private bool _useVao;
        private bool _initialized;
        private int _instanceCount;

        private static readonly float[] s_vertices =
        {
            // posX,       posY,      posZ,   u,  v
            -QuadRadius,  QuadRadius, 0.0f,  0.0f, 0.0f,
            -QuadRadius, -QuadRadius, 0.0f,  0.0f, 1.0f,
             QuadRadius, -QuadRadius, 0.0f,  1.0f, 1.0f,
             QuadRadius,  QuadRadius, 0.0f,  1.0f, 0.0f,
        };

        private static readonly ushort[] s_indices = { 0, 1, 2, 0, 2, 3 };

        private static readonly byte[] s_texture =
        {
            255,   0,   0, 255,
              0, 255,   0, 255,
              0,   0, 255, 255,
            255, 255,   0, 255,
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
            BindGeometry(gl);

            gl.Textures.ActiveTexture(TextureUnit.Texture0);
            gl.Textures.BindTexture(TextureTarget.Texture2D, _texture ?? throw new InvalidOperationException("Texture not initialized."));
            gl.Programs.Uniform1i(_samplerLoc, 0);

            gl.Draw.DrawElementsInstanced(PrimitiveType.Triangles, s_indices.Length, IndexElementType.UnsignedShort, 0, _instanceCount);
        }

        public void Dispose()
        {
            _texture?.Dispose();
            _vao?.Dispose();
            _instanceBuffer?.Dispose();
            _indexBuffer?.Dispose();
            _vertexBuffer?.Dispose();
            _programReloader?.Dispose();

            _texture = null;
            _vao = null;
            _instanceBuffer = null;
            _indexBuffer = null;
            _vertexBuffer = null;
            _gl = null;
            _initialized = false;
            _useVao = false;
            _programReloader = null;
            _lastReloadError = null;
            _instanceCount = 0;
        }

        public void SetHostContext(RendererHostContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var roots = BuildShaderRoots(context, "Demo15_SimpleInstancing.Plugin.csproj");
            _programReloader = new ShaderProgramReloader(
                Path.Combine("Shaders", "SimpleInstancing.vert.glsl"),
                Path.Combine("Shaders", "SimpleInstancing.frag.glsl"),
                roots);
        }

        private void Initialize(AngleGl gl)
        {
            if (!gl.Draw.IsInstancedArraysAvailable)
                throw new NotSupportedException("GL_ANGLE_instanced_arrays is unavailable; simple_instancing cannot run.");

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

            var instances = BuildInstances();
            _instanceCount = instances.Length / 3;
            _instanceBuffer = gl.Buffers.CreateBufferObject();
            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _instanceBuffer);
            gl.Buffers.BufferData(BufferTarget.ArrayBuffer, instances, BufferUsage.StaticDraw);

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
            gl.Textures.TexImage2D(
                TextureTarget.Texture2D,
                level: 0,
                internalFormat: TextureInternalFormat.Rgba,
                width: 2,
                height: 2,
                border: 0,
                format: PixelFormat.Rgba,
                type: PixelType.UnsignedByte,
                pixels: s_texture);
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            gl.Textures.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            gl.State.ClearColor(0f, 0f, 0f, 0f);
            _initialized = true;
        }

        private static float[] BuildInstances()
        {
            var instances = new List<float>();
            for (float y = -1.0f + QuadRadius; y < 1.0f - QuadRadius; y += QuadRadius * 3)
            {
                for (float x = -1.0f + QuadRadius; x < 1.0f - QuadRadius; x += QuadRadius * 3)
                {
                    instances.Add(x);
                    instances.Add(y);
                    instances.Add(0.0f);
                }
            }
            return instances.ToArray();
        }

        private void BindGeometry(AngleGl gl)
        {
            if (_useVao && _vao != null)
                gl.VertexArrays.BindVertexArray(_vao);

            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer ?? throw new InvalidOperationException("Vertex buffer not initialized."));
            gl.Buffers.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer ?? throw new InvalidOperationException("Index buffer not initialized."));
            gl.VertexArrays.EnableVertexAttribArray(_positionLoc);
            gl.VertexArrays.EnableVertexAttribArray(_texCoordLoc);
            gl.VertexArrays.VertexAttribPointer(_positionLoc, 3, VertexAttribType.Float, false, 5 * sizeof(float), 0);
            gl.VertexArrays.VertexAttribPointer(_texCoordLoc, 2, VertexAttribType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _instanceBuffer ?? throw new InvalidOperationException("Instance buffer not initialized."));
            gl.VertexArrays.EnableVertexAttribArray(_instanceLoc);
            gl.VertexArrays.VertexAttribPointer(_instanceLoc, 3, VertexAttribType.Float, false, 3 * sizeof(float), 0);
            gl.VertexArrays.VertexAttribDivisor(_instanceLoc, 1);
        }

        private void UpdateProgramBindings(AngleGl gl, AngleProgram program)
        {
            _positionLoc = gl.Programs.GetAttribLocation(program, "a_position");
            _texCoordLoc = gl.Programs.GetAttribLocation(program, "a_texCoord");
            _instanceLoc = gl.Programs.GetAttribLocation(program, "a_instancePos");
            _samplerLoc = gl.Programs.GetUniformLocation(program, "s_texture");

            if (_positionLoc < 0 || _texCoordLoc < 0 || _instanceLoc < 0 || _samplerLoc < 0)
                throw new InvalidOperationException("Failed to resolve shader bindings.");

            if (_useVao && _vao != null)
                BindGeometry(gl);
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
                Path.Combine("Shaders", "SimpleInstancing.vert.glsl"),
                Path.Combine("Shaders", "SimpleInstancing.frag.glsl"),
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
