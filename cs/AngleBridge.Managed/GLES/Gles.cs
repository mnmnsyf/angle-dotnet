using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AngleBridge.Managed.GLES
{
    internal static class Gles
    {
    public const uint GL_COLOR_BUFFER_BIT = 0x00004000;
    public const uint GL_DEPTH_BUFFER_BIT = 0x00000100;
    public const uint GL_STENCIL_BUFFER_BIT = 0x00000400;
    public const uint GL_TRIANGLES = 0x0004;

    public const uint GL_FLOAT = 0x1406;
    public const uint GL_UNSIGNED_BYTE = 0x1401;
    public const uint GL_UNSIGNED_SHORT = 0x1403;
    public const uint GL_RGBA = 0x1908;
    public const uint GL_STENCIL_BITS = 0x0D57;

    public const uint GL_ARRAY_BUFFER = 0x8892;
    public const uint GL_ELEMENT_ARRAY_BUFFER = 0x8893;
    public const uint GL_STATIC_DRAW = 0x88E4;

    public const uint GL_VERTEX_SHADER = 0x8B31;
    public const uint GL_FRAGMENT_SHADER = 0x8B30;
    public const uint GL_COMPILE_STATUS = 0x8B81;
    public const uint GL_LINK_STATUS = 0x8B82;
    public const uint GL_INFO_LOG_LENGTH = 0x8B84;
    public const uint GL_TIME_ELAPSED_EXT = 0x88BF;
    public const uint GL_QUERY_RESULT_EXT = 0x8866;
    public const uint GL_QUERY_RESULT_AVAILABLE_EXT = 0x8867;
    public const uint GL_GPU_DISJOINT_EXT = 0x8FBB;
    public const uint GL_DEBUG_OUTPUT = 0x92E0;
    public const uint GL_DEBUG_OUTPUT_SYNCHRONOUS = 0x8242;
    public const uint GL_CULL_FACE = 0x0B44;
    public const uint GL_DEPTH_TEST = 0x0B71;
    public const uint GL_STENCIL_TEST = 0x0B90;
    public const uint GL_BACK = 0x0405;
    public const uint GL_LESS = 0x0201;
    public const uint GL_EQUAL = 0x0202;
    public const uint GL_GREATER = 0x0204;
    public const uint GL_KEEP = 0x1E00;
    public const uint GL_REPLACE = 0x1E01;
    public const uint GL_INCR = 0x1E02;
    public const uint GL_DECR = 0x1E03;
    public const uint GL_INVERT = 0x150A;
    public const uint GL_TEXTURE_2D = 0x0DE1;
    public const uint GL_TEXTURE_CUBE_MAP = 0x8513;
    public const uint GL_TEXTURE_CUBE_MAP_POSITIVE_X = 0x8515;
    public const uint GL_TEXTURE_MIN_FILTER = 0x2801;
    public const uint GL_TEXTURE_MAG_FILTER = 0x2800;
    public const uint GL_NEAREST = 0x2600;
    public const uint GL_LINEAR = 0x2601;
    public const uint GL_LINEAR_MIPMAP_LINEAR = 0x2703;
    public const uint GL_NEAREST_MIPMAP_NEAREST = 0x2700;
    public const uint GL_UNPACK_ALIGNMENT = 0x0CF5;

    internal static CallingConvention _platformCallingConvention;

    internal static glViewport_t? _glViewport;
    internal static glClearColor_t? _glClearColor;
    internal static glClear_t? _glClear;
    internal static glClearStencil_t? _glClearStencil;
    internal static glClearDepthf_t? _glClearDepthf;
    internal static glEnable_t? _glEnable;
    internal static glStencilFunc_t? _glStencilFunc;
    internal static glStencilOp_t? _glStencilOp;
    internal static glStencilMask_t? _glStencilMask;

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
    internal static glUniformMatrix4fv_t? _glUniformMatrix4fv;

    internal static glGenBuffers_t? _glGenBuffers;
    internal static glBindBuffer_t? _glBindBuffer;
    internal static glBufferData_t? _glBufferData;
    internal static glDeleteBuffers_t? _glDeleteBuffers;

    internal static glEnableVertexAttribArray_t? _glEnableVertexAttribArray;
    internal static glVertexAttribPointer_t? _glVertexAttribPointer;
    internal static glDrawArrays_t? _glDrawArrays;
    internal static glDrawElements_t? _glDrawElements;

    internal static glGenVertexArrays_t? _glGenVertexArrays;
    internal static glBindVertexArray_t? _glBindVertexArray;
    internal static glDeleteVertexArrays_t? _glDeleteVertexArrays;

    internal static glGetError_t? _glGetError;
    internal static glReadPixels_t? _glReadPixels;
    internal static glFinish_t? _glFinish;
    internal static glGetBooleanv_t? _glGetBooleanv;
    internal static glGetIntegerv_t? _glGetIntegerv;

    internal static glGenQueriesEXT_t? _glGenQueriesEXT;
    internal static glDeleteQueriesEXT_t? _glDeleteQueriesEXT;
    internal static glBeginQueryEXT_t? _glBeginQueryEXT;
    internal static glEndQueryEXT_t? _glEndQueryEXT;
    internal static glGetQueryObjectuivEXT_t? _glGetQueryObjectuivEXT;
    internal static glGetQueryObjectui64vEXT_t? _glGetQueryObjectui64vEXT;

    internal static glDebugMessageCallback_t? _glDebugMessageCallback;
    internal static glDebugMessageControl_t? _glDebugMessageControl;

    internal static glGenTextures_t? _glGenTextures;
    internal static glDeleteTextures_t? _glDeleteTextures;
    internal static glBindTexture_t? _glBindTexture;
    internal static glTexParameteri_t? _glTexParameteri;
    internal static glTexImage2D_t? _glTexImage2D;
    internal static glPixelStorei_t? _glPixelStorei;
    internal static glGenerateMipmap_t? _glGenerateMipmap;
    internal static glCullFace_t? _glCullFace;

    public static CallingConvention CallingConvention => _platformCallingConvention;

    public static void Viewport(int x, int y, int width, int height) =>
        (_glViewport ?? throw NotLoaded(nameof(_glViewport)))(x, y, width, height);

    public static void ClearColor(float r, float g, float b, float a) =>
        (_glClearColor ?? throw NotLoaded(nameof(_glClearColor)))(r, g, b, a);

    public static void Clear(uint mask) =>
        (_glClear ?? throw NotLoaded(nameof(_glClear)))(mask);

    public static void ClearStencil(int s) =>
        (_glClearStencil ?? throw NotLoaded(nameof(_glClearStencil)))(s);

    public static void ClearDepthf(float depth) =>
        (_glClearDepthf ?? throw NotLoaded(nameof(_glClearDepthf)))(depth);

    public static void Enable(uint cap) =>
        (_glEnable ?? throw NotLoaded(nameof(_glEnable)))(cap);

    public static void StencilFunc(uint func, int @ref, uint mask) =>
        (_glStencilFunc ?? throw NotLoaded(nameof(_glStencilFunc)))(func, @ref, mask);

    public static void StencilOp(uint sfail, uint dpfail, uint dppass) =>
        (_glStencilOp ?? throw NotLoaded(nameof(_glStencilOp)))(sfail, dpfail, dppass);

    public static void StencilMask(uint mask) =>
        (_glStencilMask ?? throw NotLoaded(nameof(_glStencilMask)))(mask);

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

    public static void UniformMatrix4fv(int location, bool transpose, float[] value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        if (value.Length < 16) throw new ArgumentException("Matrix must have at least 16 elements.", nameof(value));
        var glUniformMatrix4fv = _glUniformMatrix4fv ?? throw NotLoaded(nameof(_glUniformMatrix4fv));
        var handle = GCHandle.Alloc(value, GCHandleType.Pinned);
        try
        {
            glUniformMatrix4fv(location, 1, transpose ? (byte)1 : (byte)0, handle.AddrOfPinnedObject());
        }
        finally
        {
            handle.Free();
        }
    }

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

    public static void BufferData(uint target, ushort[] data, uint usage)
    {
        var glBufferData = _glBufferData ?? throw NotLoaded(nameof(_glBufferData));
        var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        try
        {
            glBufferData(target, checked((nint)(data.Length * sizeof(ushort))), handle.AddrOfPinnedObject(), usage);
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

    public static void DrawElements(uint mode, int count, uint type, int offsetBytes)
    {
        var glDrawElements = _glDrawElements ?? throw NotLoaded(nameof(_glDrawElements));
        glDrawElements(mode, count, type, (IntPtr)offsetBytes);
    }

    public static uint GenTexture()
    {
        var glGenTextures = _glGenTextures ?? throw NotLoaded(nameof(_glGenTextures));
        glGenTextures(1, out var texture);
        return texture;
    }

    public static void DeleteTexture(uint texture)
    {
        if (texture == 0) return;
        var glDeleteTextures = _glDeleteTextures ?? throw NotLoaded(nameof(_glDeleteTextures));
        glDeleteTextures(1, ref texture);
    }

    public static void BindTexture(uint target, uint texture) =>
        (_glBindTexture ?? throw NotLoaded(nameof(_glBindTexture)))(target, texture);

    public static void TexParameteri(uint target, uint pname, int param) =>
        (_glTexParameteri ?? throw NotLoaded(nameof(_glTexParameteri)))(target, pname, param);

    public static void TexImage2D(uint target, int level, int internalFormat, int width, int height, int border, uint format, uint type, byte[]? pixels)
    {
        var glTexImage2D = _glTexImage2D ?? throw NotLoaded(nameof(_glTexImage2D));
        if (pixels == null || pixels.Length == 0)
        {
            glTexImage2D(target, level, internalFormat, width, height, border, format, type, IntPtr.Zero);
            return;
        }

        var handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
        try
        {
            glTexImage2D(target, level, internalFormat, width, height, border, format, type, handle.AddrOfPinnedObject());
        }
        finally
        {
            handle.Free();
        }
    }

    public static void PixelStorei(uint pname, int param) =>
        (_glPixelStorei ?? throw NotLoaded(nameof(_glPixelStorei)))(pname, param);

    public static void GenerateMipmap(uint target) =>
        (_glGenerateMipmap ?? throw NotLoaded(nameof(_glGenerateMipmap)))(target);

    public static void CullFace(uint mode) =>
        (_glCullFace ?? throw NotLoaded(nameof(_glCullFace)))(mode);

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

    public static bool TryGetInteger(uint pname, out int value)
    {
        var glGetIntegerv = _glGetIntegerv;
        if (glGetIntegerv is null)
        {
            value = 0;
            return false;
        }

        glGetIntegerv(pname, out value);
        return true;
    }

    public static int GetInteger(uint pname)
    {
        var glGetIntegerv = _glGetIntegerv ?? throw NotLoaded(nameof(_glGetIntegerv));
        glGetIntegerv(pname, out var v);
        return v;
    }

    public static bool TryGetBoolean(uint pname, out bool value)
    {
        var glGetBooleanv = _glGetBooleanv;
        if (glGetBooleanv is null)
        {
            value = false;
            return false;
        }

        glGetBooleanv(pname, out var v);
        value = v != 0;
        return true;
    }

    public static bool IsDisjointTimerQueryAvailable =>
        _glGenQueriesEXT != null &&
        _glDeleteQueriesEXT != null &&
        _glBeginQueryEXT != null &&
        _glEndQueryEXT != null &&
        _glGetQueryObjectuivEXT != null &&
        _glGetQueryObjectui64vEXT != null;

    public static uint GenQueryExt()
    {
        var glGenQueriesEXT = _glGenQueriesEXT ?? throw new NotSupportedException("EXT_disjoint_timer_query not available.");
        glGenQueriesEXT(1, out var id);
        return id;
    }

    public static void DeleteQueryExt(uint id)
    {
        if (id == 0) return;
        var glDeleteQueriesEXT = _glDeleteQueriesEXT ?? throw new NotSupportedException("EXT_disjoint_timer_query not available.");
        glDeleteQueriesEXT(1, ref id);
    }

    public static void BeginQueryExt(uint target, uint id)
    {
        var glBeginQueryEXT = _glBeginQueryEXT ?? throw new NotSupportedException("EXT_disjoint_timer_query not available.");
        glBeginQueryEXT(target, id);
    }

    public static void EndQueryExt(uint target)
    {
        var glEndQueryEXT = _glEndQueryEXT ?? throw new NotSupportedException("EXT_disjoint_timer_query not available.");
        glEndQueryEXT(target);
    }

    public static bool TryGetQueryResultAvailableExt(uint id, out bool available)
    {
        var glGetQueryObjectuivEXT = _glGetQueryObjectuivEXT;
        if (glGetQueryObjectuivEXT is null)
        {
            available = false;
            return false;
        }

        glGetQueryObjectuivEXT(id, GL_QUERY_RESULT_AVAILABLE_EXT, out var v);
        available = v != 0;
        return true;
    }

    public static bool TryGetQueryResultExt(uint id, out ulong result)
    {
        var glGetQueryObjectui64vEXT = _glGetQueryObjectui64vEXT;
        if (glGetQueryObjectui64vEXT is null)
        {
            result = 0;
            return false;
        }

        glGetQueryObjectui64vEXT(id, GL_QUERY_RESULT_EXT, out result);
        return true;
    }

    public static bool IsDebugCallbackAvailable =>
        _glDebugMessageCallback != null;

    public static void DebugMessageCallback(DebugProc callback, IntPtr userParam)
    {
        if (callback is null) throw new ArgumentNullException(nameof(callback));
        var glDebugMessageCallback = _glDebugMessageCallback ?? throw new NotSupportedException("GL_KHR_debug not available.");
        glDebugMessageCallback(callback, userParam);
    }

    public static void DebugMessageControl(uint source, uint type, uint severity, int count, IntPtr ids, bool enabled)
    {
        var glDebugMessageControl = _glDebugMessageControl ?? throw new NotSupportedException("GL_KHR_debug not available.");
        glDebugMessageControl(source, type, severity, count, ids, enabled ? (byte)1 : (byte)0);
    }

    internal static Exception NotLoaded(string fieldName) =>
        new InvalidOperationException($"GL entrypoint not loaded: {fieldName}. Ensure an EGL context is current, then call {nameof(GlesLoader)}.{nameof(GlesLoader.Load)}().");

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glViewport_t(int x, int y, int width, int height);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glClearColor_t(float r, float g, float b, float a);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glClear_t(uint mask);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glClearStencil_t(int s);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glClearDepthf_t(float depth);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glEnable_t(uint cap);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glStencilFunc_t(uint func, int @ref, uint mask);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glStencilOp_t(uint sfail, uint dpfail, uint dppass);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glStencilMask_t(uint mask);

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
    internal delegate void glUniformMatrix4fv_t(int location, int count, byte transpose, IntPtr value);

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
    internal delegate void glDrawElements_t(uint mode, int count, uint type, IntPtr indices);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glGenTextures_t(int n, out uint textures);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glDeleteTextures_t(int n, ref uint textures);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glBindTexture_t(uint target, uint texture);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glTexParameteri_t(uint target, uint pname, int param);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glTexImage2D_t(uint target, int level, int internalformat, int width, int height, int border, uint format, uint type, IntPtr pixels);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glPixelStorei_t(uint pname, int param);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glGenerateMipmap_t(uint target);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glCullFace_t(uint mode);

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

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glGetBooleanv_t(uint pname, out byte data);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glGetIntegerv_t(uint pname, out int data);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glGenQueriesEXT_t(int n, out uint ids);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glDeleteQueriesEXT_t(int n, ref uint ids);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glBeginQueryEXT_t(uint target, uint id);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glEndQueryEXT_t(uint target);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glGetQueryObjectuivEXT_t(uint id, uint pname, out uint paramsOut);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glGetQueryObjectui64vEXT_t(uint id, uint pname, out ulong paramsOut);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glDebugMessageCallback_t(DebugProc callback, IntPtr userParam);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glDebugMessageControl_t(uint source, uint type, uint severity, int count, IntPtr ids, byte enabled);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void DebugProc(uint source, uint type, uint id, uint severity, int length, IntPtr message, IntPtr userParam);
    }
}
