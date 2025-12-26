using AngleBridge.Sdk.HotReload;
using AngleBridge.Sdk.Interop;
using AngleBridge.Sdk.Runtime;
using System;

namespace Demo6_StencilOperations.Plugin
{
    public sealed class StencilOperationsRenderer : IRenderer
    {
        private AngleGl? _gl;
        private AngleProgram? _program;
        private AngleBuffer? _vertexBuffer;
        private AngleBuffer? _indexBuffer;
        private AngleVertexArray? _vao;
        private int _positionLoc;
        private int _colorLoc;
        private bool _useVao;
        private bool _initialized;

        private static readonly float[] s_vertices =
        {
            -0.75f,  0.25f, 0.50f,  // Quad #0
            -0.25f,  0.25f, 0.50f,
            -0.25f,  0.75f, 0.50f,
            -0.75f,  0.75f, 0.50f,

             0.25f,  0.25f, 0.90f,  // Quad #1
             0.75f,  0.25f, 0.90f,
             0.75f,  0.75f, 0.90f,
             0.25f,  0.75f, 0.90f,

            -0.75f, -0.75f, 0.50f,  // Quad #2
            -0.25f, -0.75f, 0.50f,
            -0.25f, -0.25f, 0.50f,
            -0.75f, -0.25f, 0.50f,

             0.25f, -0.75f, 0.50f,  // Quad #3
             0.75f, -0.75f, 0.50f,
             0.75f, -0.25f, 0.50f,
             0.25f, -0.25f, 0.50f,

            -1.00f, -1.00f, 0.00f,  // Big Quad
             1.00f, -1.00f, 0.00f,
             1.00f,  1.00f, 0.00f,
            -1.00f,  1.00f, 0.00f,
        };

        private static readonly ushort[] s_indices =
        {
            0, 1, 2, 0, 2, 3,        // Quad #0
            4, 5, 6, 4, 6, 7,        // Quad #1
            8, 9, 10, 8, 10, 11,     // Quad #2
            12, 13, 14, 12, 14, 15,  // Quad #3
            16, 17, 18, 16, 18, 19,  // Big Quad
        };

        private static readonly int[] s_indexOffsetsBytes =
        {
            0,
            12,
            24,
            36,
            48,
        };

        private static readonly float[,] s_colors =
        {
            { 1.0f, 0.0f, 0.0f, 1.0f },
            { 0.0f, 1.0f, 0.0f, 1.0f },
            { 0.0f, 0.0f, 1.0f, 1.0f },
            { 1.0f, 1.0f, 0.0f, 1.0f },
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

            gl.Viewport(0, 0, width, height);
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            var program = _program ?? throw new InvalidOperationException("Program not initialized.");
            gl.UseProgram(program);

            if (_useVao)
            {
                gl.BindVertexArray(_vao ?? throw new InvalidOperationException("Vertex array not initialized."));
            }
            else
            {
                BindGeometry(gl);
            }

            gl.StencilMask(0xFF);

            // Test 0
            gl.StencilFunc(StencilFunction.Less, 0x7, 0x3);
            gl.StencilOp(StencilOp.Replace, StencilOp.Decr, StencilOp.Decr);
            gl.DrawElements(PrimitiveType.Triangles, 6, IndexElementType.UnsignedShort, s_indexOffsetsBytes[0]);

            // Test 1
            gl.StencilFunc(StencilFunction.Greater, 0x3, 0x3);
            gl.StencilOp(StencilOp.Keep, StencilOp.Decr, StencilOp.Keep);
            gl.DrawElements(PrimitiveType.Triangles, 6, IndexElementType.UnsignedShort, s_indexOffsetsBytes[1]);

            // Test 2
            gl.StencilFunc(StencilFunction.Equal, 0x1, 0x3);
            gl.StencilOp(StencilOp.Keep, StencilOp.Incr, StencilOp.Incr);
            gl.DrawElements(PrimitiveType.Triangles, 6, IndexElementType.UnsignedShort, s_indexOffsetsBytes[2]);

            // Test 3
            gl.StencilFunc(StencilFunction.Equal, 0x2, 0x1);
            gl.StencilOp(StencilOp.Invert, StencilOp.Keep, StencilOp.Keep);
            gl.DrawElements(PrimitiveType.Triangles, 6, IndexElementType.UnsignedShort, s_indexOffsetsBytes[3]);

            var stencilValues = new int[4] { 0x7, 0x0, 0x2, 0x0 };
            var bits = gl.GetInteger(GetIntegerParameter.StencilBits);
            var mask = bits >= 1 && bits < 31 ? ((1 << bits) - 1) : unchecked((int)0xFF);
            stencilValues[3] = ~((mask & 0x1)) & 0xFF;

            gl.StencilMask(0x0);

            var colorLoc = _colorLoc;
            for (var i = 0; i < stencilValues.Length; i++)
            {
                gl.StencilFunc(StencilFunction.Equal, stencilValues[i], 0xFF);
                gl.Uniform4f(colorLoc, s_colors[i, 0], s_colors[i, 1], s_colors[i, 2], s_colors[i, 3]);
                gl.DrawElements(PrimitiveType.Triangles, 6, IndexElementType.UnsignedShort, s_indexOffsetsBytes[4]);
            }

            gl.StencilMask(0xFF);
        }

        public void Dispose()
        {
            if (_vao != null) _vao.Dispose();
            if (_indexBuffer != null) _indexBuffer.Dispose();
            if (_vertexBuffer != null) _vertexBuffer.Dispose();
            if (_program != null) _program.Dispose();

            _vao = null;
            _indexBuffer = null;
            _vertexBuffer = null;
            _program = null;
            _gl = null;
            _initialized = false;
            _useVao = false;
        }

        private void Initialize(AngleGl gl)
        {
            _program = CreateProgram(gl, VertexShaderSource, FragmentShaderSource);
            _positionLoc = gl.GetAttribLocation(_program, "a_position");
            _colorLoc = gl.GetUniformLocation(_program, "u_color");
            if (_positionLoc < 0 || _colorLoc < 0)
                throw new InvalidOperationException("Failed to resolve shader bindings.");

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

            gl.ClearColor(0f, 0f, 0f, 1f);
            gl.ClearStencil(0x1);
            gl.ClearDepth(0.75f);
            gl.Enable(EnableCap.DepthTest);
            gl.Enable(EnableCap.StencilTest);

            _initialized = true;
        }

        private void BindGeometry(AngleGl gl)
        {
            gl.BindBuffer(BufferTarget.ArrayBuffer, _vertexBuffer ?? throw new InvalidOperationException("Vertex buffer not initialized."));
            gl.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer ?? throw new InvalidOperationException("Index buffer not initialized."));
            gl.EnableVertexAttribArray(_positionLoc);
            gl.VertexAttribPointer(_positionLoc, 3, VertexAttribType.Float, normalized: false, stride: 3 * sizeof(float), offsetBytes: 0);
        }

        private static AngleProgram CreateProgram(AngleGl gl, string vsSource, string fsSource)
        {
            var vert = CompileShader(gl, ShaderType.VertexShader, vsSource);
            var frag = CompileShader(gl, ShaderType.FragmentShader, fsSource);

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

        private static AngleShader CompileShader(AngleGl gl, ShaderType type, string source)
        {
            var shader = gl.CreateShaderObject(type);
            gl.ShaderSource(shader, source);
            gl.CompileShader(shader);

            if (gl.GetShaderInt(shader, ShaderParameter.CompileStatus) == 0)
            {
                var log = gl.GetShaderInfoLog(shader);
                shader.Dispose();
                throw new InvalidOperationException("Shader compile failed: " + log);
            }

            return shader;
        }

        private const string VertexShaderSource =
@"#version 300 es
layout(location = 0) in vec4 a_position;
void main()
{
    gl_Position = a_position;
}";

        private const string FragmentShaderSource =
@"#version 300 es
precision mediump float;
uniform vec4 u_color;
out vec4 o_color;
void main()
{
    o_color = u_color;
}";
    }
}
