using AngleBridge.Managed.GLES;
using AngleBridge.Managed.HotReload;

namespace Demo3_Triangle.Plugin;

public sealed class TriangleRenderer : IRenderer
{
    private bool _initialized;

    private uint _program;
    private uint _vbo;
    private uint _vao;
    private bool _useVao;

    public void Draw(int width, int height)
    {
        if (!_initialized) Initialize();

        Gles.Viewport(0, 0, width, height);
        Gles.ClearColor(0.08f, 0.08f, 0.10f, 1.0f);
        Gles.Clear(Gles.GL_COLOR_BUFFER_BIT);

        Gles.UseProgram(_program);

        if (_useVao)
        {
            Gles.BindVertexArray(_vao);
        }
        else
        {
            BindGeometry();
        }

        Gles.DrawArrays(Gles.GL_TRIANGLES, first: 0, count: 3);
    }

    public void Dispose()
    {
        if (_vao != 0)
        {
            Gles.DeleteVertexArray(_vao);
            _vao = 0;
        }

        if (_vbo != 0)
        {
            Gles.DeleteBuffer(_vbo);
            _vbo = 0;
        }

        if (_program != 0)
        {
            Gles.DeleteProgram(_program);
            _program = 0;
        }

        _initialized = false;
        _useVao = false;
    }

    private void Initialize()
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
        //FragColor = vec4(0.95, 0.25, 0.20, 1.0);

        var vert = CompileShader(Gles.GL_VERTEX_SHADER, vs);
        var frag = CompileShader(Gles.GL_FRAGMENT_SHADER, fs);

        _program = LinkProgram(vert, frag);
        Gles.DeleteShader(vert);
        Gles.DeleteShader(frag);

        _vbo = Gles.GenBuffer();

        var vertices = new[]
        {
            -0.60f, -0.40f,
             0.60f, -0.40f,
             0.00f,  0.60f,
        };

        Gles.BindBuffer(Gles.GL_ARRAY_BUFFER, _vbo);
        Gles.BufferData(Gles.GL_ARRAY_BUFFER, vertices, Gles.GL_STATIC_DRAW);

        try
        {
            _vao = Gles.GenVertexArray();
            _useVao = true;
            Gles.BindVertexArray(_vao);
            BindGeometry();
        }
        catch (NotSupportedException)
        {
            _useVao = false;
        }

        _initialized = true;
    }

    private void BindGeometry()
    {
        Gles.BindBuffer(Gles.GL_ARRAY_BUFFER, _vbo);
        Gles.EnableVertexAttribArray(0);
        Gles.VertexAttribPointer(index: 0, size: 2, type: Gles.GL_FLOAT, normalized: false, stride: 2 * sizeof(float), offsetBytes: 0);
    }

    private static uint CompileShader(uint type, string source)
    {
        var shader = Gles.CreateShader(type);
        Gles.ShaderSource(shader, source);
        Gles.CompileShader(shader);

        if (Gles.GetShaderInt(shader, Gles.GL_COMPILE_STATUS) == 0)
        {
            var log = Gles.GetShaderInfoLog(shader);
            Gles.DeleteShader(shader);
            throw new InvalidOperationException($"Shader compile failed:\n{log}");
        }

        return shader;
    }

    private static uint LinkProgram(uint vert, uint frag)
    {
        var program = Gles.CreateProgram();
        Gles.AttachShader(program, vert);
        Gles.AttachShader(program, frag);
        Gles.LinkProgram(program);

        if (Gles.GetProgramInt(program, Gles.GL_LINK_STATUS) == 0)
        {
            var log = Gles.GetProgramInfoLog(program);
            Gles.DeleteProgram(program);
            throw new InvalidOperationException($"Program link failed:\n{log}");
        }

        return program;
    }
}
