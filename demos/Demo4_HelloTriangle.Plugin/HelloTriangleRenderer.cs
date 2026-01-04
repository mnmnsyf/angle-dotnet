using AngleBridge.Sdk.HotReload;
using AngleBridge.Sdk.Interop;
using AngleBridge.Sdk.Runtime;
using System;

namespace Demo4_HelloTriangle.Plugin
{
    public sealed class HelloTriangleRenderer : IRenderer
    {
        private AngleGl? _gl;
        private AngleProgram? _program;
        private AngleBuffer? _vbo;
        private int _positionLoc;
        private bool _initialized;

        private static readonly float[] s_vertices =
        {
            0.0f, 0.5f, 0.0f,
            -0.5f, -0.5f, 0.0f,
            0.5f, -0.5f, 0.0f,
        };

        private const string VertexShaderSource =
"attribute vec4 vPosition;\n" +
"void main()\n" +
"{\n" +
"    gl_Position = vPosition;\n" +
"}\n";

        private const string FragmentShaderSource =
"precision mediump float;\n" +
"void main()\n" +
"{\n" +
"    gl_FragColor = vec4(1.0, 0.0, 0.0, 1.0);\n" +
"}\n";

        public void Draw(AngleGl gl, int width, int height)
        {
            if (gl == null) throw new ArgumentNullException(nameof(gl));
            if (_gl == null)
                _gl = gl;
            else if (!ReferenceEquals(_gl, gl))
                throw new InvalidOperationException("Renderer cannot be shared across different AngleGl instances.");

            if (!_initialized)
                Initialize(gl);

            gl.State.Viewport(0, 0, width, height);
            gl.State.Clear(ClearBufferMask.ColorBufferBit);

            gl.Programs.UseProgram(_program ?? throw new InvalidOperationException("Program not initialized."));
            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _vbo ?? throw new InvalidOperationException("Buffer not initialized."));
            gl.VertexArrays.VertexAttribPointer(_positionLoc, 3, VertexAttribType.Float, false, 0, 0);
            gl.VertexArrays.EnableVertexAttribArray(_positionLoc);
            gl.Draw.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }

        public void Dispose()
        {
            if (_vbo != null) _vbo.Dispose();
            if (_program != null) _program.Dispose();

            _vbo = null;
            _program = null;
            _initialized = false;
            _gl = null;
        }

        private void Initialize(AngleGl gl)
        {
            _program = CreateProgram(gl, VertexShaderSource, FragmentShaderSource);
            _positionLoc = gl.Programs.GetAttribLocation(_program, "vPosition");
            if (_positionLoc < 0)
                throw new InvalidOperationException("Failed to resolve shader attribute location.");

            _vbo = gl.Buffers.CreateBufferObject();
            gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            gl.Buffers.BufferData(BufferTarget.ArrayBuffer, s_vertices, BufferUsage.StaticDraw);

            gl.State.ClearColor(0f, 0f, 0f, 1f);
            _initialized = true;
        }

        private static AngleProgram CreateProgram(AngleGl gl, string vsSource, string fsSource)
        {
            var vert = CompileShader(gl, ShaderType.VertexShader, vsSource);
            var frag = CompileShader(gl, ShaderType.FragmentShader, fsSource);

            var program = gl.Programs.CreateProgramObject();
            gl.Programs.AttachShader(program, vert);
            gl.Programs.AttachShader(program, frag);
            gl.Programs.LinkProgram(program);

            vert.Dispose();
            frag.Dispose();

            if (gl.Programs.GetProgramInt(program, ProgramParameter.LinkStatus) == 0)
            {
                var log = gl.Programs.GetProgramInfoLog(program);
                program.Dispose();
                throw new InvalidOperationException("Program link failed: " + log);
            }

            return program;
        }

        private static AngleShader CompileShader(AngleGl gl, ShaderType type, string source)
        {
            var shader = gl.Shaders.CreateShaderObject(type);
            gl.Shaders.ShaderSource(shader, source);
            gl.Shaders.CompileShader(shader);

            if (gl.Shaders.GetShaderInt(shader, ShaderParameter.CompileStatus) == 0)
            {
                var log = gl.Shaders.GetShaderInfoLog(shader);
                shader.Dispose();
                throw new InvalidOperationException("Shader compile failed: " + log);
            }

            return shader;
        }
    }
}
