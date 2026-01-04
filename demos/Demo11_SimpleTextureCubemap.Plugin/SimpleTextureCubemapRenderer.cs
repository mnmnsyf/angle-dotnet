using AngleBridge.Sdk.HotReload;
using AngleBridge.Sdk.Interop;
using AngleBridge.Sdk.Runtime;
using System;
using System.IO;

namespace Demo11_SimpleTextureCubemap.Plugin
{
    public sealed class SimpleTextureCubemapRenderer : IRenderer, IRendererWithHostContext
    {
        private AngleGl? _gl;
        private AngleBuffer? _vertexBuffer;
        private AngleBuffer? _indexBuffer;
        private AngleTexture? _texture;
        private ShaderProgramReloader? _programReloader;
        private Exception? _lastReloadError;
        private bool _initialized;
        private int _positionLoc;
        private int _normalLoc;
        private int _samplerLoc;
        private int _indexCount;

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

            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer ?? throw new InvalidOperationException("Vertex buffer not initialized."));
            gl.Buffers.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer ?? throw new InvalidOperationException("Index buffer not initialized."));

            gl.VertexArrays.VertexAttribPointer(_positionLoc, 3, VertexAttribType.Float, false, 6 * sizeof(float), 0);
            gl.VertexArrays.VertexAttribPointer(_normalLoc, 3, VertexAttribType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            gl.VertexArrays.EnableVertexAttribArray(_positionLoc);
            gl.VertexArrays.EnableVertexAttribArray(_normalLoc);

            gl.Textures.ActiveTexture(TextureUnit.Texture0);
            gl.Textures.BindTexture(TextureTarget.TextureCubeMap, _texture ?? throw new InvalidOperationException("Texture not initialized."));
            gl.Programs.Uniform1i(_samplerLoc, 0);

            gl.Draw.DrawElements(PrimitiveType.Triangles, _indexCount, IndexElementType.UnsignedShort, 0);
        }

        public void Dispose()
        {
            _texture?.Dispose();
            _indexBuffer?.Dispose();
            _vertexBuffer?.Dispose();
            _programReloader?.Dispose();

            _texture = null;
            _indexBuffer = null;
            _vertexBuffer = null;
            _programReloader = null;
            _lastReloadError = null;
            _gl = null;
            _initialized = false;
            _indexCount = 0;
        }

        private void Initialize(AngleGl gl)
        {
            EnsureProgramReloader();
            var reloader = _programReloader ?? throw new InvalidOperationException("Program reloader not initialized.");
            reloader.Load(gl);
            UpdateProgramBindings(gl, reloader.Current);

            CreateSphereGeometry(slices: 128, radius: 0.75f, out var vertices, out var indices);
            _indexCount = indices.Length;

            _vertexBuffer = gl.Buffers.CreateBufferObject();
            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer);
            gl.Buffers.BufferData(BufferTarget.ArrayBuffer, vertices, BufferUsage.StaticDraw);

            _indexBuffer = gl.Buffers.CreateBufferObject();
            gl.Buffers.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
            gl.Buffers.BufferData(BufferTarget.ElementArrayBuffer, indices, BufferUsage.StaticDraw);

            _texture = gl.Textures.CreateTextureObject();
            gl.Textures.ActiveTexture(TextureUnit.Texture0);
            gl.Textures.BindTexture(TextureTarget.TextureCubeMap, _texture);
            gl.Textures.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            UploadCubemapFaces(gl);
            gl.Textures.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            gl.Textures.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            gl.State.ClearColor(0f, 0f, 0f, 0f);
            gl.State.CullFace(CullFaceMode.Back);
            gl.State.Enable(EnableCap.CullFace);

            _initialized = true;
        }

        private void UpdateProgramBindings(AngleGl gl, AngleProgram program)
        {
            _positionLoc = gl.Programs.GetAttribLocation(program, "a_position");
            _normalLoc = gl.Programs.GetAttribLocation(program, "a_normal");
            _samplerLoc = gl.Programs.GetUniformLocation(program, "s_texture");
            if (_positionLoc < 0 || _normalLoc < 0 || _samplerLoc < 0)
                throw new InvalidOperationException("Failed to resolve shader bindings.");
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
                Path.Combine("Shaders", "SimpleTextureCubemap.vert.glsl"),
                Path.Combine("Shaders", "SimpleTextureCubemap.frag.glsl"),
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
                Path.Combine("Shaders", "SimpleTextureCubemap.vert.glsl"),
                Path.Combine("Shaders", "SimpleTextureCubemap.frag.glsl"),
                roots);
        }

        private static void UploadCubemapFaces(AngleGl gl)
        {
            var pixels = new[]
            {
                new byte[] { 255, 0, 0 },     // +X red
                new byte[] { 0, 255, 0 },     // -X green
                new byte[] { 0, 0, 255 },     // +Y blue
                new byte[] { 255, 255, 0 },   // -Y yellow
                new byte[] { 255, 0, 255 },   // +Z purple
                new byte[] { 255, 255, 255 }, // -Z white
            };

            var targets = new[]
            {
                TextureTarget.TextureCubeMapPositiveX,
                TextureTarget.TextureCubeMapNegativeX,
                TextureTarget.TextureCubeMapPositiveY,
                TextureTarget.TextureCubeMapNegativeY,
                TextureTarget.TextureCubeMapPositiveZ,
                TextureTarget.TextureCubeMapNegativeZ,
            };

            for (var i = 0; i < 6; i++)
            {
                gl.Textures.TexImage2D(targets[i], 0, TextureInternalFormat.Rgb, 1, 1, 0, PixelFormat.Rgb, PixelType.UnsignedByte, pixels[i]);
            }
        }

        private static void CreateSphereGeometry(int slices, float radius, out float[] vertices, out ushort[] indices)
        {
            if (slices < 3) throw new ArgumentOutOfRangeException(nameof(slices));
            if (radius <= 0) throw new ArgumentOutOfRangeException(nameof(radius));

            var parallelCount = slices / 2;
            var vertexCount = (parallelCount + 1) * (slices + 1);
            var angleStep = (MathF.PI * 2f) / slices;

            vertices = new float[vertexCount * 6];
            var vi = 0;
            for (var i = 0; i < parallelCount + 1; i++)
            {
                for (var j = 0; j < slices + 1; j++)
                {
                    var sinI = MathF.Sin(angleStep * i);
                    var x = sinI * MathF.Sin(angleStep * j);
                    var y = MathF.Cos(angleStep * i);
                    var z = sinI * MathF.Cos(angleStep * j);

                    vertices[vi++] = x * radius;
                    vertices[vi++] = y * radius;
                    vertices[vi++] = z * radius;
                    vertices[vi++] = x;
                    vertices[vi++] = y;
                    vertices[vi++] = z;
                }
            }

            indices = new ushort[parallelCount * slices * 6];
            var ii = 0;
            for (var i = 0; i < parallelCount; i++)
            {
                for (var j = 0; j < slices; j++)
                {
                    var a = i * (slices + 1) + j;
                    var b = (i + 1) * (slices + 1) + j;
                    var c = (i + 1) * (slices + 1) + (j + 1);
                    var d = i * (slices + 1) + (j + 1);

                    indices[ii++] = (ushort)a;
                    indices[ii++] = (ushort)b;
                    indices[ii++] = (ushort)c;
                    indices[ii++] = (ushort)a;
                    indices[ii++] = (ushort)c;
                    indices[ii++] = (ushort)d;
                }
            }
        }
    }
}
