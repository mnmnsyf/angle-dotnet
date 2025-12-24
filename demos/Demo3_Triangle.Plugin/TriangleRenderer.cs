using AngleBridge.Sdk.HotReload;
using AngleBridge.Sdk.Interop;
using AngleBridge.Sdk.Runtime;

namespace Demo3_Triangle.Plugin
{
    public sealed class TriangleRenderer : IRenderer
    {
    private bool _initialized;

    private AngleGl? _gl;

    private AngleProgram? _program;
    private AngleBuffer? _vbo;
    private AngleVertexArray? _vao;
    private bool _useVao;

    public void Draw(AngleGl gl, int width, int height)
    {
        if (gl == null) throw new ArgumentNullException(nameof(gl));
        if (_gl == null)
            _gl = gl;
        else if (!ReferenceEquals(_gl, gl))
            throw new InvalidOperationException("Renderer cannot be shared across different AngleGl instances.");

        if (!_initialized) Initialize(gl);

        gl.Viewport(0, 0, width, height);
        gl.ClearColor(0.08f, 0.08f, 0.10f, 1.0f);
        gl.Clear(ClearBufferMask.ColorBufferBit);

        var program = _program ?? throw new InvalidOperationException("Program not initialized.");
        gl.UseProgram(program);

        if (_useVao)
        {
            var vao = _vao ?? throw new InvalidOperationException("Vertex array not initialized.");
            gl.BindVertexArray(vao);
        }
        else
        {
            BindGeometry(gl);
        }

        gl.DrawArrays(PrimitiveType.Triangles, first: 0, count: 3);
    }

    public void Dispose()
    {
        if (_vao != null) _vao.Dispose();
        if (_vbo != null) _vbo.Dispose();
        if (_program != null) _program.Dispose();

        _vao = null;
        _vbo = null;
        _program = null;

        _initialized = false;
        _useVao = false;
    }

    private void Initialize(AngleGl gl)
    {
        const string vs =
            "#version 300 es\n" +
            "layout(location = 0) in vec2 aPos;\n" +
            "void main() {\n" +
            "  gl_Position = vec4(aPos, 0.0, 1.0);\n" +
            "}\n";

        const string fs =
            "#version 300 es\n" +
            "precision mediump float;\n" +
            "out vec4 FragColor;\n" +
            "void main() {\n" +
            "  FragColor = vec4(0.95, 0.25, 0.20, 1.0);\n" +
            "}\n";
        //FragColor = vec4(0.1, 0.1, 0.1, 1.0);

        var vert = CompileShader(gl, ShaderType.VertexShader, vs);
        var frag = CompileShader(gl, ShaderType.FragmentShader, fs);

        var program = LinkProgram(gl, vert, frag);
        vert.Dispose();
        frag.Dispose();

        _program = program;

        _vbo = gl.CreateBufferObject();

        var vertices = new[]
        {
            -0.60f, -0.40f,
             0.60f, -0.40f,
             0.00f,  0.60f,
        };

        gl.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        gl.BufferData(BufferTarget.ArrayBuffer, vertices, BufferUsage.StaticDraw);

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
            if (_vao != null)
            {
                _vao.Dispose();
                _vao = null;
            }
        }

        _initialized = true;
    }

    private void BindGeometry(AngleGl gl)
    {
        var vbo = _vbo ?? throw new InvalidOperationException("Buffer not initialized.");
        gl.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        gl.EnableVertexAttribArray(0);
        gl.VertexAttribPointer(index: 0, size: 2, type: VertexAttribType.Float, normalized: false, stride: 2 * sizeof(float), offsetBytes: 0);
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
            throw new InvalidOperationException($"Shader compile failed:\n{log}");
        }

        return shader;
    }

    private static AngleProgram LinkProgram(AngleGl gl, AngleShader vert, AngleShader frag)
    {
        var program = gl.CreateProgramObject();
        gl.AttachShader(program, vert);
        gl.AttachShader(program, frag);
        gl.LinkProgram(program);

        if (gl.GetProgramInt(program, ProgramParameter.LinkStatus) == 0)
        {
            var log = gl.GetProgramInfoLog(program);
            program.Dispose();
            throw new InvalidOperationException($"Program link failed:\n{log}");
        }

        return program;
    }
}
}
