using AngleBridge.Sdk.HotReload;
using AngleBridge.Sdk.Interop;
using AngleBridge.Sdk.Runtime;
using System;
using System.Diagnostics;
using System.IO;

namespace Demo12_ParticleSystem.Plugin
{
    public sealed class ParticleSystemRenderer : IRenderer, IRendererWithHostContext
    {
        private const int ParticleCount = 1024;

        private AngleGl? _gl;
        private AngleBuffer? _particleBuffer;
        private AngleTexture? _texture;
        private ShaderProgramReloader? _programReloader;
        private Exception? _lastReloadError;
        private bool _initialized;

        private int _lifetimeLoc;
        private int _startPositionLoc;
        private int _endPositionLoc;
        private int _timeLoc;
        private int _centerPositionLoc;
        private int _colorLoc;
        private int _samplerLoc;

        private readonly Random _rng = new Random(1234);
        private readonly Stopwatch _timer = Stopwatch.StartNew();
        private double _prevTime;
        private float _particleTime = 1.0f;

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

            var now = _timer.Elapsed.TotalSeconds;
            var dt = (float)(now - _prevTime);
            _prevTime = now;

            gl.UseProgram(reloader.Current);
            Step(gl, dt);

            gl.Viewport(0, 0, width, height);
            gl.Clear(ClearBufferMask.ColorBufferBit);

            gl.BindBuffer(BufferTarget.ArrayBuffer, _particleBuffer ?? throw new InvalidOperationException("Particle buffer not initialized."));
            gl.VertexAttribPointer(_lifetimeLoc, 1, VertexAttribType.Float, false, 7 * sizeof(float), 0);
            gl.VertexAttribPointer(_startPositionLoc, 3, VertexAttribType.Float, false, 7 * sizeof(float), 1 * sizeof(float));
            gl.VertexAttribPointer(_endPositionLoc, 3, VertexAttribType.Float, false, 7 * sizeof(float), 4 * sizeof(float));
            gl.EnableVertexAttribArray(_lifetimeLoc);
            gl.EnableVertexAttribArray(_startPositionLoc);
            gl.EnableVertexAttribArray(_endPositionLoc);

            gl.Enable(EnableCap.Blend);
            gl.BlendFunc(BlendFactor.SrcAlpha, BlendFactor.One);

            gl.ActiveTexture(TextureUnit.Texture0);
            gl.BindTexture(TextureTarget.Texture2D, _texture ?? throw new InvalidOperationException("Texture not initialized."));
            gl.Uniform1i(_samplerLoc, 0);

            gl.DrawArrays(PrimitiveType.Points, 0, ParticleCount);
        }

        public void Dispose()
        {
            _texture?.Dispose();
            _particleBuffer?.Dispose();
            _programReloader?.Dispose();

            _texture = null;
            _particleBuffer = null;
            _programReloader = null;
            _lastReloadError = null;
            _gl = null;
            _initialized = false;
        }

        private void Initialize(AngleGl gl)
        {
            EnsureProgramReloader();
            var reloader = _programReloader ?? throw new InvalidOperationException("Program reloader not initialized.");
            reloader.Load(gl);
            UpdateProgramBindings(gl, reloader.Current);

            _particleBuffer = gl.CreateBufferObject();
            gl.BindBuffer(BufferTarget.ArrayBuffer, _particleBuffer);
            gl.BufferData(BufferTarget.ArrayBuffer, CreateParticles(), BufferUsage.StaticDraw);

            _texture = gl.CreateTextureObject();
            gl.ActiveTexture(TextureUnit.Texture0);
            gl.BindTexture(TextureTarget.Texture2D, _texture);
            gl.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            UploadSmokeTexture(gl, size: 64);
            gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            gl.ClearColor(0f, 0f, 0f, 0f);
            ResetEmitter(gl);
            _initialized = true;
        }

        private void Step(AngleGl gl, float dt)
        {
            _particleTime += dt;
            if (_particleTime >= 1.0f)
            {
                _particleTime = 0.0f;
                ResetEmitter(gl);
            }

            gl.Uniform1f(_timeLoc, _particleTime);
        }

        private void ResetEmitter(AngleGl gl)
        {
            var cx = NextFloat(-0.5f, 0.5f);
            var cy = NextFloat(-0.5f, 0.5f);
            var cz = NextFloat(-0.5f, 0.5f);
            gl.Uniform3f(_centerPositionLoc, cx, cy, cz);

            var r = NextFloat(0.0f, 1.0f);
            var g = NextFloat(0.0f, 1.0f);
            var b = NextFloat(0.0f, 1.0f);
            gl.Uniform4f(_colorLoc, r, g, b, 0.5f);
        }

        private float NextFloat(float min, float max)
        {
            return min + (float)_rng.NextDouble() * (max - min);
        }

        private static float[] CreateParticles()
        {
            var rng = new Random(1234);
            var data = new float[ParticleCount * 7];
            var i = 0;
            for (var p = 0; p < ParticleCount; p++)
            {
                var lifetime = (float)rng.NextDouble();

                var endAngle = (float)(rng.NextDouble() * (MathF.PI * 2f));
                var endRadius = (float)(rng.NextDouble() * 2.0);
                var endX = MathF.Sin(endAngle) * endRadius;
                var endY = MathF.Cos(endAngle) * endRadius;

                var startAngle = (float)(rng.NextDouble() * (MathF.PI * 2f));
                var startRadius = (float)(rng.NextDouble() * 0.25);
                var startX = MathF.Sin(startAngle) * startRadius;
                var startY = MathF.Cos(startAngle) * startRadius;

                data[i++] = lifetime;
                data[i++] = startX;
                data[i++] = startY;
                data[i++] = 0f;
                data[i++] = endX;
                data[i++] = endY;
                data[i++] = 0f;
            }

            return data;
        }

        private static void UploadSmokeTexture(AngleGl gl, int size)
        {
            var pixels = new byte[size * size * 4];
            var idx = 0;
            var inv = 1.0f / (size - 1);

            for (var y = 0; y < size; y++)
            {
                var fy = (y * inv) * 2f - 1f;
                for (var x = 0; x < size; x++)
                {
                    var fx = (x * inv) * 2f - 1f;
                    var r = MathF.Sqrt(fx * fx + fy * fy);

                    var a = 1.0f - MathF.Min(1.0f, r);
                    a = a * a;
                    var alpha = (byte)(a * 255.0f);
                    pixels[idx++] = 255;
                    pixels[idx++] = 255;
                    pixels[idx++] = 255;
                    pixels[idx++] = alpha;
                }
            }

            gl.TexImage2D(TextureTarget.Texture2D, 0, TextureInternalFormat.Rgba, size, size, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
        }

        private void UpdateProgramBindings(AngleGl gl, AngleProgram program)
        {
            _lifetimeLoc = gl.GetAttribLocation(program, "a_lifetime");
            _startPositionLoc = gl.GetAttribLocation(program, "a_startPosition");
            _endPositionLoc = gl.GetAttribLocation(program, "a_endPosition");
            _timeLoc = gl.GetUniformLocation(program, "u_time");
            _centerPositionLoc = gl.GetUniformLocation(program, "u_centerPosition");
            _colorLoc = gl.GetUniformLocation(program, "u_color");
            _samplerLoc = gl.GetUniformLocation(program, "s_texture");

            if (_lifetimeLoc < 0 || _startPositionLoc < 0 || _endPositionLoc < 0 ||
                _timeLoc < 0 || _centerPositionLoc < 0 || _colorLoc < 0 || _samplerLoc < 0)
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
                Path.Combine("Shaders", "ParticleSystem.vert.glsl"),
                Path.Combine("Shaders", "ParticleSystem.frag.glsl"),
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
                Path.Combine("Shaders", "ParticleSystem.vert.glsl"),
                Path.Combine("Shaders", "ParticleSystem.frag.glsl"),
                roots);
        }
    }
}

