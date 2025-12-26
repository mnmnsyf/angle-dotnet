using AngleBridge.Sdk.Interop;
using AngleBridge.Sdk.Runtime;
using System;

namespace AngleBridge.Sdk.HotReload
{
    public sealed class ShaderProgramReloader : IDisposable
    {
        private readonly ShaderFile _vertexShader;
        private readonly ShaderFile _fragmentShader;

        private AngleGl? _gl;
        private AngleProgram? _program;
        private string? _vertexSource;
        private string? _fragmentSource;

        public ShaderProgramReloader(string vertexPath, string fragmentPath, params string[] searchRoots)
        {
            _vertexShader = new ShaderFile(vertexPath, searchRoots);
            _fragmentShader = new ShaderFile(fragmentPath, searchRoots);
        }

        public ShaderProgramReloader(ShaderFile vertexShader, ShaderFile fragmentShader)
        {
            _vertexShader = vertexShader ?? throw new ArgumentNullException(nameof(vertexShader));
            _fragmentShader = fragmentShader ?? throw new ArgumentNullException(nameof(fragmentShader));
        }

        public AngleProgram Current
        {
            get
            {
                if (_program == null)
                    throw new InvalidOperationException("Program not loaded yet. Call Load() first.");
                return _program;
            }
        }

        public Exception? LastError { get; private set; }

        public bool IsLoaded { get { return _program != null; } }

        public void Load(AngleGl gl)
        {
            if (gl == null) throw new ArgumentNullException(nameof(gl));
            EnsureSameContext(gl);

            var vertexSource = _vertexShader.Load();
            var fragmentSource = _fragmentShader.Load();

            var program = CreateProgram(gl, vertexSource, fragmentSource, _vertexShader.ResolvedPath, _fragmentShader.ResolvedPath);
            _program?.Dispose();
            _program = program;
            _vertexSource = vertexSource;
            _fragmentSource = fragmentSource;
            LastError = null;
        }

        public bool TryReload(AngleGl gl)
        {
            if (gl == null) throw new ArgumentNullException(nameof(gl));
            EnsureSameContext(gl);

            if (_program == null)
            {
                try
                {
                    Load(gl);
                    return true;
                }
                catch (Exception ex)
                {
                    LastError = ex;
                    return false;
                }
            }

            var currentVertex = _vertexSource;
            var currentFragment = _fragmentSource;
            if (currentVertex == null || currentFragment == null)
            {
                try
                {
                    Load(gl);
                    return true;
                }
                catch (Exception ex)
                {
                    LastError = ex;
                    return false;
                }
            }

            var changed = false;
            try
            {
                if (_vertexShader.TryReload(out var newVertex))
                {
                    currentVertex = newVertex;
                    changed = true;
                }
                else if (_vertexShader.LastError != null)
                {
                    LastError = _vertexShader.LastError;
                    return false;
                }

                if (_fragmentShader.TryReload(out var newFragment))
                {
                    currentFragment = newFragment;
                    changed = true;
                }
                else if (_fragmentShader.LastError != null)
                {
                    LastError = _fragmentShader.LastError;
                    return false;
                }
            }
            catch (Exception ex)
            {
                LastError = ex;
                return false;
            }

            if (!changed)
                return false;

            try
            {
                var newProgram = CreateProgram(gl, currentVertex, currentFragment, _vertexShader.ResolvedPath, _fragmentShader.ResolvedPath);
                _program.Dispose();
                _program = newProgram;
                _vertexSource = currentVertex;
                _fragmentSource = currentFragment;
                LastError = null;
                return true;
            }
            catch (Exception ex)
            {
                LastError = ex;
                return false;
            }
        }

        public void Dispose()
        {
            if (_program != null)
            {
                _program.Dispose();
                _program = null;
            }
        }

        private void EnsureSameContext(AngleGl gl)
        {
            if (_gl == null)
            {
                _gl = gl;
                return;
            }

            if (!ReferenceEquals(_gl, gl))
                throw new InvalidOperationException("Shader reloader cannot be shared across different AngleGl instances.");
        }

        private static AngleProgram CreateProgram(AngleGl gl, string vsSource, string fsSource, string vertexPath, string fragmentPath)
        {
            var vert = CompileShader(gl, ShaderType.VertexShader, NormalizeSource(vsSource, vertexPath), vertexPath);
            var frag = CompileShader(gl, ShaderType.FragmentShader, NormalizeSource(fsSource, fragmentPath), fragmentPath);

            var program = gl.CreateProgramObject();
            gl.AttachShader(program, vert);
            gl.AttachShader(program, frag);
            gl.LinkProgram(program);

            vert.Dispose();
            frag.Dispose();

            if (gl.GetProgramInt(program, ProgramParameter.LinkStatus) == 0)
            {
                var log = gl.GetProgramInfoLog(program);
                program.Dispose();
                throw new InvalidOperationException("Program link failed: " + log);
            }

            return program;
        }

        private static AngleShader CompileShader(AngleGl gl, ShaderType type, string source, string shaderPath)
        {
            var shader = gl.CreateShaderObject(type);
            gl.ShaderSource(shader, source);
            gl.CompileShader(shader);

            if (gl.GetShaderInt(shader, ShaderParameter.CompileStatus) == 0)
            {
                var log = gl.GetShaderInfoLog(shader);
                shader.Dispose();
                throw new InvalidOperationException("Shader compile failed (" + shaderPath + "): " + log);
            }

            return shader;
        }

        private static string NormalizeSource(string source, string shaderPath)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrWhiteSpace(source))
                throw new InvalidOperationException("Shader source is empty: " + shaderPath);

            if (source.Length > 0 && source[0] == '\uFEFF')
                source = source.Substring(1);

            var trimmed = source.TrimStart();
            if (trimmed.StartsWith("#version", StringComparison.Ordinal) &&
                !source.StartsWith("#version", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("GLSL #version must be the first characters in the file (no BOM/leading whitespace): " + shaderPath);
            }

            return source;
        }
    }
}
