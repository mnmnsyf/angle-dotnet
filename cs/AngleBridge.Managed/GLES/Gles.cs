using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AngleBridge.Managed.GLES;

public static class Gles
{
    public const uint GL_COLOR_BUFFER_BIT = 0x00004000;
    public const uint GL_TRIANGLES = 0x0004;

    public const uint GL_FLOAT = 0x1406;
    public const uint GL_UNSIGNED_BYTE = 0x1401;
    public const uint GL_RGBA = 0x1908;

    public const uint GL_ARRAY_BUFFER = 0x8892;
    public const uint GL_STATIC_DRAW = 0x88E4;

    public const uint GL_VERTEX_SHADER = 0x8B31;
    public const uint GL_FRAGMENT_SHADER = 0x8B30;
    public const uint GL_COMPILE_STATUS = 0x8B81;
    public const uint GL_LINK_STATUS = 0x8B82;
    public const uint GL_INFO_LOG_LENGTH = 0x8B84;

    internal static CallingConvention _platformCallingConvention;

    internal static glViewport_t? _glViewport;
    internal static glClearColor_t? _glClearColor;
    internal static glClear_t? _glClear;

    internal static glCreateShader_t? _glCreateShader;
    internal static glShaderSource_t? _glShaderSource;
    internal static glCompileShader_t? _glCompileShader;
    internal static glGetShaderiv_t? _glGetShaderiv;
    internal static glGetShaderInfoLog_t? _glGetShaderInfoLog;
    internal static glDeleteShader_t? _glDeleteShader;

    internal static glCreateProgram_t? _glCreateProgram;
    internal static glAttachShader_t? _glAttachShader;
    internal static glLinkProgram_t? _glLinkProgram;
    internal static glGetProgramiv_t? _glGetProgramiv;
    internal static glGetProgramInfoLog_t? _glGetProgramInfoLog;
    internal static glUseProgram_t? _glUseProgram;
    internal static glDeleteProgram_t? _glDeleteProgram;

    internal static glGetAttribLocation_t? _glGetAttribLocation;
    internal static glGetUniformLocation_t? _glGetUniformLocation;
    internal static glUniform4f_t? _glUniform4f;

    internal static glGenBuffers_t? _glGenBuffers;
    internal static glBindBuffer_t? _glBindBuffer;
    internal static glBufferData_t? _glBufferData;
    internal static glDeleteBuffers_t? _glDeleteBuffers;

    internal static glEnableVertexAttribArray_t? _glEnableVertexAttribArray;
    internal static glVertexAttribPointer_t? _glVertexAttribPointer;
    internal static glDrawArrays_t? _glDrawArrays;

    internal static glGenVertexArrays_t? _glGenVertexArrays;
    internal static glBindVertexArray_t? _glBindVertexArray;
    internal static glDeleteVertexArrays_t? _glDeleteVertexArrays;

    internal static glGetError_t? _glGetError;
    internal static glReadPixels_t? _glReadPixels;
    internal static glFinish_t? _glFinish;

    public static CallingConvention CallingConvention => _platformCallingConvention;

    public static void Viewport(int x, int y, int width, int height) =>
        (_glViewport ?? throw NotLoaded(nameof(_glViewport)))(x, y, width, height);

    public static void ClearColor(float r, float g, float b, float a) =>
        (_glClearColor ?? throw NotLoaded(nameof(_glClearColor)))(r, g, b, a);

    public static void Clear(uint mask) =>
        (_glClear ?? throw NotLoaded(nameof(_glClear)))(mask);

    public static uint CreateShader(uint type) =>
        (_glCreateShader ?? throw NotLoaded(nameof(_glCreateShader)))(type);

    public static void ShaderSource(uint shader, string source)
    {
        var glShaderSource = _glShaderSource ?? throw NotLoaded(nameof(_glShaderSource));

        var utf8 = Marshal.StringToCoTaskMemUTF8(source);
        var strings = IntPtr.Zero;
        var lengths = IntPtr.Zero;
        try
        {
            strings = Marshal.AllocHGlobal(IntPtr.Size);
            Marshal.WriteIntPtr(strings, utf8);

            lengths = Marshal.AllocHGlobal(sizeof(int));
            Marshal.WriteInt32(lengths, Encoding.UTF8.GetByteCount(source));

            glShaderSource(shader, 1, strings, lengths);
        }
        finally
        {
            if (lengths != IntPtr.Zero) Marshal.FreeHGlobal(lengths);
            if (strings != IntPtr.Zero) Marshal.FreeHGlobal(strings);
            Marshal.FreeCoTaskMem(utf8);
        }
    }

    public static void CompileShader(uint shader) =>
        (_glCompileShader ?? throw NotLoaded(nameof(_glCompileShader)))(shader);

    public static int GetShaderInt(uint shader, uint pname)
    {
        var glGetShaderiv = _glGetShaderiv ?? throw NotLoaded(nameof(_glGetShaderiv));
        glGetShaderiv(shader, pname, out var v);
        return v;
    }

    public static string GetShaderInfoLog(uint shader)
    {
        var glGetShaderInfoLog = _glGetShaderInfoLog ?? throw NotLoaded(nameof(_glGetShaderInfoLog));
        var len = GetShaderInt(shader, GL_INFO_LOG_LENGTH);
        if (len <= 1) return string.Empty;

        var buf = Marshal.AllocHGlobal(len);
        try
        {
            glGetShaderInfoLog(shader, len, out var written, buf);
            return Marshal.PtrToStringUTF8(buf, written) ?? string.Empty;
        }
        finally
        {
            Marshal.FreeHGlobal(buf);
        }
    }

    public static void DeleteShader(uint shader) =>
        (_glDeleteShader ?? throw NotLoaded(nameof(_glDeleteShader)))(shader);

    public static uint CreateProgram() =>
        (_glCreateProgram ?? throw NotLoaded(nameof(_glCreateProgram)))();

    public static void AttachShader(uint program, uint shader) =>
        (_glAttachShader ?? throw NotLoaded(nameof(_glAttachShader)))(program, shader);

    public static void LinkProgram(uint program) =>
        (_glLinkProgram ?? throw NotLoaded(nameof(_glLinkProgram)))(program);

    public static int GetProgramInt(uint program, uint pname)
    {
        var glGetProgramiv = _glGetProgramiv ?? throw NotLoaded(nameof(_glGetProgramiv));
        glGetProgramiv(program, pname, out var v);
        return v;
    }

    public static string GetProgramInfoLog(uint program)
    {
        var glGetProgramInfoLog = _glGetProgramInfoLog ?? throw NotLoaded(nameof(_glGetProgramInfoLog));
        var len = GetProgramInt(program, GL_INFO_LOG_LENGTH);
        if (len <= 1) return string.Empty;

        var buf = Marshal.AllocHGlobal(len);
        try
        {
            glGetProgramInfoLog(program, len, out var written, buf);
            return Marshal.PtrToStringUTF8(buf, written) ?? string.Empty;
        }
        finally
        {
            Marshal.FreeHGlobal(buf);
        }
    }

    public static void UseProgram(uint program) =>
        (_glUseProgram ?? throw NotLoaded(nameof(_glUseProgram)))(program);

    public static void DeleteProgram(uint program) =>
        (_glDeleteProgram ?? throw NotLoaded(nameof(_glDeleteProgram)))(program);

    public static int GetAttribLocation(uint program, string name) =>
        (_glGetAttribLocation ?? throw NotLoaded(nameof(_glGetAttribLocation)))(program, name);

    public static int GetUniformLocation(uint program, string name) =>
        (_glGetUniformLocation ?? throw NotLoaded(nameof(_glGetUniformLocation)))(program, name);

    public static void Uniform4f(int location, float v0, float v1, float v2, float v3) =>
        (_glUniform4f ?? throw NotLoaded(nameof(_glUniform4f)))(location, v0, v1, v2, v3);

    public static uint GenBuffer()
    {
        var glGenBuffers = _glGenBuffers ?? throw NotLoaded(nameof(_glGenBuffers));
        glGenBuffers(1, out var buffer);
        return buffer;
    }

    public static void DeleteBuffer(uint buffer)
    {
        if (buffer == 0) return;
        var glDeleteBuffers = _glDeleteBuffers ?? throw NotLoaded(nameof(_glDeleteBuffers));
        glDeleteBuffers(1, ref buffer);
    }

    public static void BindBuffer(uint target, uint buffer) =>
        (_glBindBuffer ?? throw NotLoaded(nameof(_glBindBuffer)))(target, buffer);

    public static void BufferData(uint target, float[] data, uint usage)
    {
        var glBufferData = _glBufferData ?? throw NotLoaded(nameof(_glBufferData));
        var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        try
        {
            glBufferData(target, checked((nint)(data.Length * sizeof(float))), handle.AddrOfPinnedObject(), usage);
        }
        finally
        {
            handle.Free();
        }
    }

    public static void EnableVertexAttribArray(uint index) =>
        (_glEnableVertexAttribArray ?? throw NotLoaded(nameof(_glEnableVertexAttribArray)))(index);

    public static void VertexAttribPointer(uint index, int size, uint type, bool normalized, int stride, int offsetBytes)
    {
        var glVertexAttribPointer = _glVertexAttribPointer ?? throw NotLoaded(nameof(_glVertexAttribPointer));
        glVertexAttribPointer(index, size, type, normalized ? (byte)1 : (byte)0, stride, (IntPtr)offsetBytes);
    }

    public static void DrawArrays(uint mode, int first, int count) =>
        (_glDrawArrays ?? throw NotLoaded(nameof(_glDrawArrays)))(mode, first, count);

    public static uint GenVertexArray()
    {
        var glGenVertexArrays = _glGenVertexArrays ?? throw new NotSupportedException("glGenVertexArrays not available (need GLES3).");
        glGenVertexArrays(1, out var vao);
        return vao;
    }

    public static void BindVertexArray(uint vao) =>
        (_glBindVertexArray ?? throw new NotSupportedException("glBindVertexArray not available (need GLES3)."))(vao);

    public static void DeleteVertexArray(uint vao)
    {
        if (vao == 0) return;
        var glDeleteVertexArrays = _glDeleteVertexArrays;
        if (glDeleteVertexArrays is null) return;
        glDeleteVertexArrays(1, ref vao);
    }

    public static uint GetError() =>
        (_glGetError ?? throw NotLoaded(nameof(_glGetError)))();

    public static void ReadPixels(int x, int y, int width, int height, byte[] rgba)
    {
        if (rgba is null) throw new ArgumentNullException(nameof(rgba));
        var expected = checked(width * height * 4);
        if (rgba.Length < expected)
            throw new ArgumentException($"Buffer too small for {width}x{height} RGBA.", nameof(rgba));

        var glReadPixels = _glReadPixels ?? throw NotLoaded(nameof(_glReadPixels));
        var handle = GCHandle.Alloc(rgba, GCHandleType.Pinned);
        try
        {
            glReadPixels(x, y, width, height, GL_RGBA, GL_UNSIGNED_BYTE, handle.AddrOfPinnedObject());
        }
        finally
        {
            handle.Free();
        }
    }

    public static void Finish() =>
        (_glFinish ?? throw NotLoaded(nameof(_glFinish)))();

    internal static Exception NotLoaded(string fieldName) =>
        new InvalidOperationException($"GL entrypoint not loaded: {fieldName}. Ensure an EGL context is current, then call {nameof(GlesLoader)}.{nameof(GlesLoader.Load)}().");

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glViewport_t(int x, int y, int width, int height);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glClearColor_t(float r, float g, float b, float a);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glClear_t(uint mask);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate uint glCreateShader_t(uint type);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glShaderSource_t(uint shader, int count, IntPtr strings, IntPtr lengths);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glCompileShader_t(uint shader);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glGetShaderiv_t(uint shader, uint pname, out int param);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glGetShaderInfoLog_t(uint shader, int bufSize, out int length, IntPtr infoLog);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glDeleteShader_t(uint shader);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate uint glCreateProgram_t();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glAttachShader_t(uint program, uint shader);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glLinkProgram_t(uint program);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glGetProgramiv_t(uint program, uint pname, out int param);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glGetProgramInfoLog_t(uint program, int bufSize, out int length, IntPtr infoLog);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glUseProgram_t(uint program);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glDeleteProgram_t(uint program);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int glGetAttribLocation_t(uint program, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate int glGetUniformLocation_t(uint program, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glUniform4f_t(int location, float v0, float v1, float v2, float v3);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glGenBuffers_t(int n, out uint buffers);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glBindBuffer_t(uint target, uint buffer);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glBufferData_t(uint target, nint size, IntPtr data, uint usage);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glDeleteBuffers_t(int n, ref uint buffers);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glEnableVertexAttribArray_t(uint index);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glVertexAttribPointer_t(uint index, int size, uint type, byte normalized, int stride, IntPtr pointer);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glDrawArrays_t(uint mode, int first, int count);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glGenVertexArrays_t(int n, out uint arrays);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glBindVertexArray_t(uint array);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glDeleteVertexArrays_t(int n, ref uint arrays);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate uint glGetError_t();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glReadPixels_t(int x, int y, int width, int height, uint format, uint type, IntPtr data);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glFinish_t();
}
