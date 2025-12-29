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
    public const uint GL_FRAMEBUFFER = 0x8D40;
    public const uint GL_READ_FRAMEBUFFER = 0x8CA8;
    public const uint GL_DRAW_FRAMEBUFFER = 0x8CA9;
    public const uint GL_RENDERBUFFER = 0x8D41;
    public const uint GL_COLOR_ATTACHMENT0 = 0x8CE0;
    public const uint GL_DEPTH_ATTACHMENT = 0x8D00;
    public const uint GL_STENCIL_ATTACHMENT = 0x8D20;
    public const uint GL_DEPTH_STENCIL_ATTACHMENT = 0x821A;
    public const uint GL_FRAMEBUFFER_COMPLETE = 0x8CD5;
    public const uint GL_FRAMEBUFFER_UNSUPPORTED = 0x8CDD;
    public const uint GL_RENDERBUFFER_WIDTH = 0x8D42;
    public const uint GL_RENDERBUFFER_HEIGHT = 0x8D43;
    public const uint GL_RENDERBUFFER_INTERNAL_FORMAT = 0x8D44;
    public const uint GL_RENDERBUFFER_SAMPLES = 0x8CAB;
    public const uint GL_FRAMEBUFFER_BINDING = 0x8CA6;
    public const uint GL_READ_FRAMEBUFFER_BINDING = 0x8CAA;
    public const uint GL_DRAW_FRAMEBUFFER_BINDING = 0x8CA6;
    public const uint GL_RENDERBUFFER_BINDING = 0x8CA7;
    public const uint GL_DEPTH_COMPONENT16 = 0x81A5;
    public const uint GL_DEPTH_COMPONENT24 = 0x81A6;
    public const uint GL_STENCIL_INDEX8 = 0x8D48;
    public const uint GL_DEPTH24_STENCIL8 = 0x88F0;
    public const uint GL_DEPTH_COMPONENT = 0x1902;
    public const uint GL_STENCIL_INDEX = 0x1901;
    public const uint GL_DEPTH_STENCIL = 0x84F9;
    public const uint GL_UNSIGNED_INT_24_8 = 0x84FA;
    public const uint GL_UNSIGNED_INT = 0x1405;
    public const uint GL_SAMPLER_BINDING = 0x8919;
    public const uint GL_TEXTURE_WRAP_S = 0x2802;
    public const uint GL_TEXTURE_WRAP_T = 0x2803;
    public const uint GL_TEXTURE_WRAP_R = 0x8072;
    public const uint GL_CLAMP_TO_EDGE = 0x812F;
    public const uint GL_MIRRORED_REPEAT = 0x8370;
    public const uint GL_REPEAT = 0x2901;
    public const uint GL_QUERY_RESULT = 0x8866;
    public const uint GL_QUERY_RESULT_AVAILABLE = 0x8867;
    public const uint GL_TIME_ELAPSED = 0x88BF;
    public const uint GL_ANY_SAMPLES_PASSED = 0x8C2F;
    public const uint GL_TRANSFORM_FEEDBACK_PRIMITIVES_WRITTEN = 0x8C88;
    public const uint GL_TRANSFORM_FEEDBACK = 0x8E22;
    public const uint GL_TRANSFORM_FEEDBACK_BUFFER = 0x8C8E;
    public const uint GL_TRANSFORM_FEEDBACK_BUFFER_BINDING = 0x8C8F;
    public const uint GL_TRANSFORM_FEEDBACK_BUFFER_BASE = 0x8C8C;
    public const uint GL_TRANSFORM_FEEDBACK_BUFFER_SIZE = 0x8C85;
    public const uint GL_INTERLEAVED_ATTRIBS = 0x8C8C;
    public const uint GL_SEPARATE_ATTRIBS = 0x8C8D;
    public const uint GL_SYNC_GPU_COMMANDS_COMPLETE = 0x9117;
    public const ulong GL_TIMEOUT_IGNORED = 0xFFFFFFFFFFFFFFFF;
    public const uint GL_ALREADY_SIGNALED = 0x911A;
    public const uint GL_TIMEOUT_EXPIRED = 0x911B;
    public const uint GL_CONDITION_SATISFIED = 0x911C;
    public const uint GL_WAIT_FAILED = 0x911D;
    public const uint GL_SYNC_STATUS = 0x9114;
    public const uint GL_OBJECT_TYPE = 0x9112;
    public const uint GL_SYNC_CONDITION = 0x9113;
    public const uint GL_SYNC_FLAGS = 0x9115;
    public const uint GL_SYNC_FENCE = 0x9116;
    public const uint GL_SYNC_FLUSH_COMMANDS_BIT = 0x00000001;
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
    internal static glGenFramebuffers_t? _glGenFramebuffers;
    internal static glDeleteFramebuffers_t? _glDeleteFramebuffers;
    internal static glBindFramebuffer_t? _glBindFramebuffer;
    internal static glFramebufferTexture2D_t? _glFramebufferTexture2D;
    internal static glFramebufferRenderbuffer_t? _glFramebufferRenderbuffer;
    internal static glCheckFramebufferStatus_t? _glCheckFramebufferStatus;
    internal static glGenRenderbuffers_t? _glGenRenderbuffers;
    internal static glDeleteRenderbuffers_t? _glDeleteRenderbuffers;
    internal static glBindRenderbuffer_t? _glBindRenderbuffer;
    internal static glRenderbufferStorage_t? _glRenderbufferStorage;
    internal static glBlitFramebuffer_t? _glBlitFramebuffer;
    internal static glGenSamplers_t? _glGenSamplers;
    internal static glDeleteSamplers_t? _glDeleteSamplers;
    internal static glBindSampler_t? _glBindSampler;
    internal static glSamplerParameteri_t? _glSamplerParameteri;
    internal static glSamplerParameterf_t? _glSamplerParameterf;
    internal static glGenQueries_t? _glGenQueries;
    internal static glDeleteQueries_t? _glDeleteQueries;
    internal static glBeginQuery_t? _glBeginQuery;
    internal static glEndQuery_t? _glEndQuery;
    internal static glGetQueryObjectuiv_t? _glGetQueryObjectuiv;
    internal static glGetQueryObjectui64v_t? _glGetQueryObjectui64v;
    internal static glFenceSync_t? _glFenceSync;
    internal static glDeleteSync_t? _glDeleteSync;
    internal static glClientWaitSync_t? _glClientWaitSync;
    internal static glWaitSync_t? _glWaitSync;
    internal static glIsSync_t? _glIsSync;
    internal static glGetSynciv_t? _glGetSynciv;
    internal static glGenTransformFeedbacks_t? _glGenTransformFeedbacks;
    internal static glDeleteTransformFeedbacks_t? _glDeleteTransformFeedbacks;
    internal static glBindTransformFeedback_t? _glBindTransformFeedback;
    internal static glBeginTransformFeedback_t? _glBeginTransformFeedback;
    internal static glEndTransformFeedback_t? _glEndTransformFeedback;
    internal static glPauseTransformFeedback_t? _glPauseTransformFeedback;
    internal static glResumeTransformFeedback_t? _glResumeTransformFeedback;
    internal static glTransformFeedbackVaryings_t? _glTransformFeedbackVaryings;
    internal static glGetTransformFeedbackVarying_t? _glGetTransformFeedbackVarying;
    internal static glBindBufferBase_t? _glBindBufferBase;
    internal static glBindBufferRange_t? _glBindBufferRange;

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

    public static uint GenFramebuffer()
    {
        var glGenFramebuffers = _glGenFramebuffers ?? throw NotLoaded(nameof(_glGenFramebuffers));
        glGenFramebuffers(1, out var id);
        return id;
    }

    public static void DeleteFramebuffer(uint framebuffer)
    {
        if (framebuffer == 0) return;
        var glDeleteFramebuffers = _glDeleteFramebuffers;
        if (glDeleteFramebuffers is null) return;
        glDeleteFramebuffers(1, ref framebuffer);
    }

    public static void BindFramebuffer(uint target, uint framebuffer) =>
        (_glBindFramebuffer ?? throw NotLoaded(nameof(_glBindFramebuffer)))(target, framebuffer);

    public static void FramebufferTexture2D(uint target, uint attachment, uint textarget, uint texture, int level) =>
        (_glFramebufferTexture2D ?? throw NotLoaded(nameof(_glFramebufferTexture2D)))(target, attachment, textarget, texture, level);

    public static void FramebufferRenderbuffer(uint target, uint attachment, uint renderbufferTarget, uint renderbuffer) =>
        (_glFramebufferRenderbuffer ?? throw NotLoaded(nameof(_glFramebufferRenderbuffer)))(target, attachment, renderbufferTarget, renderbuffer);

    public static uint CheckFramebufferStatus(uint target) =>
        (_glCheckFramebufferStatus ?? throw NotLoaded(nameof(_glCheckFramebufferStatus)))(target);

    public static uint GenRenderbuffer()
    {
        var glGenRenderbuffers = _glGenRenderbuffers ?? throw NotLoaded(nameof(_glGenRenderbuffers));
        glGenRenderbuffers(1, out var id);
        return id;
    }

    public static void DeleteRenderbuffer(uint renderbuffer)
    {
        if (renderbuffer == 0) return;
        var glDeleteRenderbuffers = _glDeleteRenderbuffers;
        if (glDeleteRenderbuffers is null) return;
        glDeleteRenderbuffers(1, ref renderbuffer);
    }

    public static void BindRenderbuffer(uint target, uint renderbuffer) =>
        (_glBindRenderbuffer ?? throw NotLoaded(nameof(_glBindRenderbuffer)))(target, renderbuffer);

    public static void RenderbufferStorage(uint target, uint internalFormat, int width, int height) =>
        (_glRenderbufferStorage ?? throw NotLoaded(nameof(_glRenderbufferStorage)))(target, internalFormat, width, height);

    public static void BlitFramebuffer(
        int srcX0, int srcY0, int srcX1, int srcY1,
        int dstX0, int dstY0, int dstX1, int dstY1,
        uint mask, uint filter) =>
        (_glBlitFramebuffer ?? throw NotLoaded(nameof(_glBlitFramebuffer)))(srcX0, srcY0, srcX1, srcY1, dstX0, dstY0, dstX1, dstY1, mask, filter);

    public static uint GenSampler()
    {
        var glGenSamplers = _glGenSamplers ?? throw NotSupported(nameof(_glGenSamplers), "Samplers require GLES3.");
        glGenSamplers(1, out var id);
        return id;
    }

    public static void DeleteSampler(uint sampler)
    {
        if (sampler == 0) return;
        var glDeleteSamplers = _glDeleteSamplers;
        if (glDeleteSamplers is null) return;
        glDeleteSamplers(1, ref sampler);
    }

    public static void BindSampler(uint unit, uint sampler)
    {
        var glBindSampler = _glBindSampler ?? throw NotSupported(nameof(_glBindSampler), "Samplers require GLES3.");
        glBindSampler(unit, sampler);
    }

    public static void SamplerParameteri(uint sampler, uint pname, int param)
    {
        var glSamplerParameteri = _glSamplerParameteri ?? throw NotSupported(nameof(_glSamplerParameteri), "Samplers require GLES3.");
        glSamplerParameteri(sampler, pname, param);
    }

    public static void SamplerParameterf(uint sampler, uint pname, float param)
    {
        var glSamplerParameterf = _glSamplerParameterf ?? throw NotSupported(nameof(_glSamplerParameterf), "Samplers require GLES3.");
        glSamplerParameterf(sampler, pname, param);
    }

    public static uint GenQuery()
    {
        if (_glGenQueries != null)
        {
            _glGenQueries(1, out var id);
            return id;
        }

        var ext = _glGenQueriesEXT ?? throw NotSupported(nameof(_glGenQueries), "Queries not available.");
        ext(1, out var idExt);
        return idExt;
    }

    public static void DeleteQuery(uint query)
    {
        if (query == 0) return;
        if (_glDeleteQueries != null)
        {
            _glDeleteQueries(1, ref query);
            return;
        }

        var ext = _glDeleteQueriesEXT;
        if (ext != null) ext(1, ref query);
    }

    public static void BeginQuery(uint target, uint id)
    {
        if (_glBeginQuery != null)
        {
            _glBeginQuery(target, id);
            return;
        }

        var ext = _glBeginQueryEXT ?? throw NotSupported(nameof(_glBeginQuery), "Queries not available.");
        ext(target, id);
    }

    public static void EndQuery(uint target)
    {
        if (_glEndQuery != null)
        {
            _glEndQuery(target);
            return;
        }

        var ext = _glEndQueryEXT ?? throw NotSupported(nameof(_glEndQuery), "Queries not available.");
        ext(target);
    }

    public static bool TryGetQueryObjectuiv(uint id, uint pname, out uint value)
    {
        if (_glGetQueryObjectuiv != null)
        {
            _glGetQueryObjectuiv(id, pname, out value);
            return true;
        }

        var ext = _glGetQueryObjectuivEXT;
        if (ext is null)
        {
            value = 0;
            return false;
        }
        ext(id, pname, out value);
        return true;
    }

    public static bool TryGetQueryObjectui64v(uint id, uint pname, out ulong value)
    {
        if (_glGetQueryObjectui64v != null)
        {
            _glGetQueryObjectui64v(id, pname, out value);
            return true;
        }

        var ext = _glGetQueryObjectui64vEXT;
        if (ext is null)
        {
            value = 0;
            return false;
        }
        ext(id, pname, out value);
        return true;
    }

    public static IntPtr FenceSync(uint condition, uint flags)
    {
        var glFenceSync = _glFenceSync ?? throw NotSupported(nameof(_glFenceSync), "Sync not available.");
        return glFenceSync(condition, flags);
    }

    public static void DeleteSync(IntPtr sync)
    {
        if (sync == IntPtr.Zero) return;
        var glDeleteSync = _glDeleteSync;
        if (glDeleteSync is null) return;
        glDeleteSync(sync);
    }

    public static uint ClientWaitSync(IntPtr sync, uint flags, ulong timeout)
    {
        var glClientWaitSync = _glClientWaitSync ?? throw NotSupported(nameof(_glClientWaitSync), "Sync not available.");
        return glClientWaitSync(sync, flags, timeout);
    }

    public static void WaitSync(IntPtr sync, uint flags, ulong timeout)
    {
        var glWaitSync = _glWaitSync ?? throw NotSupported(nameof(_glWaitSync), "Sync not available.");
        glWaitSync(sync, flags, timeout);
    }

    public static bool IsSync(IntPtr sync)
    {
        var glIsSync = _glIsSync ?? throw NotSupported(nameof(_glIsSync), "Sync not available.");
        return glIsSync(sync) != 0;
    }

    public static bool TryGetSynciv(IntPtr sync, uint pname, int bufSize, int[] values, out int length)
    {
        var glGetSynciv = _glGetSynciv;
        if (glGetSynciv is null)
        {
            length = 0;
            return false;
        }

        if (values == null) throw new ArgumentNullException(nameof(values));
        if (bufSize > values.Length) throw new ArgumentOutOfRangeException(nameof(bufSize), "bufSize exceeds array length.");

        var handle = GCHandle.Alloc(values, GCHandleType.Pinned);
        try
        {
            glGetSynciv(sync, pname, bufSize, out length, handle.AddrOfPinnedObject());
        }
        finally
        {
            handle.Free();
        }

        return true;
    }

    public static uint GenTransformFeedback()
    {
        var glGenTransformFeedbacks = _glGenTransformFeedbacks ?? throw NotSupported(nameof(_glGenTransformFeedbacks), "Transform feedback requires GLES3.");
        glGenTransformFeedbacks(1, out var id);
        return id;
    }

    public static void DeleteTransformFeedback(uint id)
    {
        if (id == 0) return;
        var glDeleteTransformFeedbacks = _glDeleteTransformFeedbacks;
        if (glDeleteTransformFeedbacks is null) return;
        glDeleteTransformFeedbacks(1, ref id);
    }

    public static void BindTransformFeedback(uint target, uint id)
    {
        var glBindTransformFeedback = _glBindTransformFeedback ?? throw NotSupported(nameof(_glBindTransformFeedback), "Transform feedback requires GLES3.");
        glBindTransformFeedback(target, id);
    }

    public static void BeginTransformFeedback(uint primitiveMode)
    {
        var glBeginTransformFeedback = _glBeginTransformFeedback ?? throw NotSupported(nameof(_glBeginTransformFeedback), "Transform feedback requires GLES3.");
        glBeginTransformFeedback(primitiveMode);
    }

    public static void EndTransformFeedback()
    {
        var glEndTransformFeedback = _glEndTransformFeedback ?? throw NotSupported(nameof(_glEndTransformFeedback), "Transform feedback requires GLES3.");
        glEndTransformFeedback();
    }

    public static void PauseTransformFeedback()
    {
        var glPauseTransformFeedback = _glPauseTransformFeedback ?? throw NotSupported(nameof(_glPauseTransformFeedback), "Transform feedback requires GLES3.");
        glPauseTransformFeedback();
    }

    public static void ResumeTransformFeedback()
    {
        var glResumeTransformFeedback = _glResumeTransformFeedback ?? throw NotSupported(nameof(_glResumeTransformFeedback), "Transform feedback requires GLES3.");
        glResumeTransformFeedback();
    }

    public static void TransformFeedbackVaryings(uint program, int count, string[] varyings, uint bufferMode)
    {
        var glTransformFeedbackVaryings = _glTransformFeedbackVaryings ?? throw NotSupported(nameof(_glTransformFeedbackVaryings), "Transform feedback requires GLES3.");
        if (varyings == null) throw new ArgumentNullException(nameof(varyings));
        if (count < 0 || count > varyings.Length) throw new ArgumentOutOfRangeException(nameof(count));

        var ptrs = new IntPtr[count];
        var handles = new GCHandle[count];
        try
        {
            for (var i = 0; i < count; i++)
            {
                handles[i] = GCHandle.Alloc(Encoding.UTF8.GetBytes(varyings[i] + "\0"), GCHandleType.Pinned);
                ptrs[i] = handles[i].AddrOfPinnedObject();
            }

            var ptrArray = GCHandle.Alloc(ptrs, GCHandleType.Pinned);
            try
            {
                glTransformFeedbackVaryings(program, count, ptrArray.AddrOfPinnedObject(), bufferMode);
            }
            finally
            {
                ptrArray.Free();
            }
        }
        finally
        {
            for (var i = 0; i < count; i++)
            {
                if (handles[i].IsAllocated) handles[i].Free();
            }
        }
    }

    public static bool TryGetTransformFeedbackVarying(uint program, uint index, int bufSize, out int length, out int size, out uint type, byte[]? nameUtf8)
    {
        var glGetTransformFeedbackVarying = _glGetTransformFeedbackVarying;
        if (glGetTransformFeedbackVarying is null)
        {
            length = 0;
            size = 0;
            type = 0;
            return false;
        }

        IntPtr namePtr = IntPtr.Zero;
        GCHandle handle = default;
        if (nameUtf8 != null && nameUtf8.Length > 0)
        {
            handle = GCHandle.Alloc(nameUtf8, GCHandleType.Pinned);
            namePtr = handle.AddrOfPinnedObject();
        }

        try
        {
            glGetTransformFeedbackVarying(program, index, bufSize, out length, out size, out type, namePtr);
        }
        finally
        {
            if (handle.IsAllocated) handle.Free();
        }

        return true;
    }

    public static void BindBufferBase(uint target, uint index, uint buffer)
    {
        var glBindBufferBase = _glBindBufferBase ?? throw NotSupported(nameof(_glBindBufferBase), "BindBufferBase requires GLES3.");
        glBindBufferBase(target, index, buffer);
    }

    public static void BindBufferRange(uint target, uint index, uint buffer, nint offset, nint size)
    {
        var glBindBufferRange = _glBindBufferRange ?? throw NotSupported(nameof(_glBindBufferRange), "BindBufferRange requires GLES3.");
        glBindBufferRange(target, index, buffer, offset, size);
    }

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

    internal static Exception NotSupported(string fieldName, string reason) =>
        new NotSupportedException($"{fieldName}: {reason}");

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
    internal delegate void glGenFramebuffers_t(int n, out uint ids);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glDeleteFramebuffers_t(int n, ref uint ids);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glBindFramebuffer_t(uint target, uint framebuffer);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glFramebufferTexture2D_t(uint target, uint attachment, uint textarget, uint texture, int level);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glFramebufferRenderbuffer_t(uint target, uint attachment, uint renderbufferTarget, uint renderbuffer);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate uint glCheckFramebufferStatus_t(uint target);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glGenRenderbuffers_t(int n, out uint ids);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glDeleteRenderbuffers_t(int n, ref uint ids);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glBindRenderbuffer_t(uint target, uint renderbuffer);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glRenderbufferStorage_t(uint target, uint internalFormat, int width, int height);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glBlitFramebuffer_t(int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, uint mask, uint filter);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glGenSamplers_t(int n, out uint samplers);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glDeleteSamplers_t(int n, ref uint samplers);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glBindSampler_t(uint unit, uint sampler);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glSamplerParameteri_t(uint sampler, uint pname, int param);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glSamplerParameterf_t(uint sampler, uint pname, float param);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glGenQueries_t(int n, out uint ids);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glDeleteQueries_t(int n, ref uint ids);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glBeginQuery_t(uint target, uint id);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glEndQuery_t(uint target);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glGetQueryObjectuiv_t(uint id, uint pname, out uint paramsOut);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glGetQueryObjectui64v_t(uint id, uint pname, out ulong paramsOut);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate IntPtr glFenceSync_t(uint condition, uint flags);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glDeleteSync_t(IntPtr sync);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate uint glClientWaitSync_t(IntPtr sync, uint flags, ulong timeout);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glWaitSync_t(IntPtr sync, uint flags, ulong timeout);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate byte glIsSync_t(IntPtr sync);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glGetSynciv_t(IntPtr sync, uint pname, int bufSize, out int length, IntPtr values);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glGenTransformFeedbacks_t(int n, out uint ids);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glDeleteTransformFeedbacks_t(int n, ref uint ids);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glBindTransformFeedback_t(uint target, uint id);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glBeginTransformFeedback_t(uint primitiveMode);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glEndTransformFeedback_t();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glPauseTransformFeedback_t();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glResumeTransformFeedback_t();

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glTransformFeedbackVaryings_t(uint program, int count, IntPtr varyings, uint bufferMode);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glGetTransformFeedbackVarying_t(uint program, uint index, int bufSize, out int length, out int size, out uint type, IntPtr name);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glBindBufferBase_t(uint target, uint index, uint buffer);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glBindBufferRange_t(uint target, uint index, uint buffer, nint offset, nint size);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glDebugMessageCallback_t(DebugProc callback, IntPtr userParam);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void glDebugMessageControl_t(uint source, uint type, uint severity, int count, IntPtr ids, byte enabled);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate void DebugProc(uint source, uint type, uint id, uint severity, int length, IntPtr message, IntPtr userParam);
    }
}
