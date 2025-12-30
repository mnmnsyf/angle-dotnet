using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace AngleBridge.Managed.GLES
{
    internal static class GlesLoader
    {
        private const int GL_VERSION = 0x1F02;

        private static int _loaded;
        private static readonly object s_lock = new object();

        public static CallingConvention PlatformCallingConvention =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? CallingConvention.StdCall
                : CallingConvention.Cdecl;

        public static bool IsLoaded => Volatile.Read(ref _loaded) != 0;

        public static void Load()
        {
            if (IsLoaded) return;

            lock (s_lock)
            {
                if (IsLoaded) return;

                var versionPtr = global::AngleBridgeNative.AB_GetGLString(GL_VERSION);
                if (versionPtr == IntPtr.Zero)
                    throw new InvalidOperationException("No current EGL context. Call Load() only after creating the EGL context and making it current.");

                Gles._platformCallingConvention = PlatformCallingConvention;

                Gles._glViewport = LoadFunction<Gles.glViewport_t>("glViewport");
                Gles._glClearColor = LoadFunction<Gles.glClearColor_t>("glClearColor");
                Gles._glClear = LoadFunction<Gles.glClear_t>("glClear");
                Gles._glClearStencil = LoadFunction<Gles.glClearStencil_t>("glClearStencil");
                Gles._glClearDepthf = LoadFunction<Gles.glClearDepthf_t>("glClearDepthf");
                Gles._glEnable = LoadFunction<Gles.glEnable_t>("glEnable");
                Gles._glStencilFunc = LoadFunction<Gles.glStencilFunc_t>("glStencilFunc");
                Gles._glStencilOp = LoadFunction<Gles.glStencilOp_t>("glStencilOp");
                Gles._glStencilMask = LoadFunction<Gles.glStencilMask_t>("glStencilMask");

                Gles._glCreateShader = LoadFunction<Gles.glCreateShader_t>("glCreateShader");
                Gles._glShaderSource = LoadFunction<Gles.glShaderSource_t>("glShaderSource");
                Gles._glCompileShader = LoadFunction<Gles.glCompileShader_t>("glCompileShader");
                Gles._glGetShaderiv = LoadFunction<Gles.glGetShaderiv_t>("glGetShaderiv");
                Gles._glGetShaderInfoLog = LoadFunction<Gles.glGetShaderInfoLog_t>("glGetShaderInfoLog");
                Gles._glDeleteShader = LoadFunction<Gles.glDeleteShader_t>("glDeleteShader");

                Gles._glCreateProgram = LoadFunction<Gles.glCreateProgram_t>("glCreateProgram");
                Gles._glAttachShader = LoadFunction<Gles.glAttachShader_t>("glAttachShader");
                Gles._glLinkProgram = LoadFunction<Gles.glLinkProgram_t>("glLinkProgram");
                Gles._glGetProgramiv = LoadFunction<Gles.glGetProgramiv_t>("glGetProgramiv");
                Gles._glGetProgramInfoLog = LoadFunction<Gles.glGetProgramInfoLog_t>("glGetProgramInfoLog");
                Gles._glUseProgram = LoadFunction<Gles.glUseProgram_t>("glUseProgram");
                Gles._glDeleteProgram = LoadFunction<Gles.glDeleteProgram_t>("glDeleteProgram");

                Gles._glGetAttribLocation = LoadFunction<Gles.glGetAttribLocation_t>("glGetAttribLocation");
                Gles._glGetUniformLocation = LoadFunction<Gles.glGetUniformLocation_t>("glGetUniformLocation");
                Gles._glUniform1i = LoadFunction<Gles.glUniform1i_t>("glUniform1i");
                Gles._glUniform4f = LoadFunction<Gles.glUniform4f_t>("glUniform4f");
                Gles._glUniformMatrix4fv = LoadFunction<Gles.glUniformMatrix4fv_t>("glUniformMatrix4fv");

                Gles._glGenBuffers = LoadFunction<Gles.glGenBuffers_t>("glGenBuffers");
                Gles._glBindBuffer = LoadFunction<Gles.glBindBuffer_t>("glBindBuffer");
                Gles._glBufferData = LoadFunction<Gles.glBufferData_t>("glBufferData");
                Gles._glDeleteBuffers = LoadFunction<Gles.glDeleteBuffers_t>("glDeleteBuffers");

                Gles._glEnableVertexAttribArray = LoadFunction<Gles.glEnableVertexAttribArray_t>("glEnableVertexAttribArray");
                Gles._glVertexAttribPointer = LoadFunction<Gles.glVertexAttribPointer_t>("glVertexAttribPointer");
                Gles._glDrawArrays = LoadFunction<Gles.glDrawArrays_t>("glDrawArrays");
                Gles._glDrawElements = LoadFunction<Gles.glDrawElements_t>("glDrawElements");

                Gles._glGenVertexArrays = TryLoadFunction<Gles.glGenVertexArrays_t>("glGenVertexArrays");
                Gles._glBindVertexArray = TryLoadFunction<Gles.glBindVertexArray_t>("glBindVertexArray");
                Gles._glDeleteVertexArrays = TryLoadFunction<Gles.glDeleteVertexArrays_t>("glDeleteVertexArrays");

                Gles._glGetError = TryLoadFunction<Gles.glGetError_t>("glGetError");
                Gles._glReadPixels = LoadFunction<Gles.glReadPixels_t>("glReadPixels");
                Gles._glFinish = LoadFunction<Gles.glFinish_t>("glFinish");
                Gles._glGetBooleanv = TryLoadFunction<Gles.glGetBooleanv_t>("glGetBooleanv");
                Gles._glGetIntegerv = TryLoadFunction<Gles.glGetIntegerv_t>("glGetIntegerv");

                Gles._glGenQueriesEXT = TryLoadFunction<Gles.glGenQueriesEXT_t>("glGenQueriesEXT");
                Gles._glDeleteQueriesEXT = TryLoadFunction<Gles.glDeleteQueriesEXT_t>("glDeleteQueriesEXT");
                Gles._glBeginQueryEXT = TryLoadFunction<Gles.glBeginQueryEXT_t>("glBeginQueryEXT");
                Gles._glEndQueryEXT = TryLoadFunction<Gles.glEndQueryEXT_t>("glEndQueryEXT");
                Gles._glGetQueryObjectuivEXT = TryLoadFunction<Gles.glGetQueryObjectuivEXT_t>("glGetQueryObjectuivEXT");
                Gles._glGetQueryObjectui64vEXT = TryLoadFunction<Gles.glGetQueryObjectui64vEXT_t>("glGetQueryObjectui64vEXT");

                Gles._glGenTextures = LoadFunction<Gles.glGenTextures_t>("glGenTextures");
                Gles._glDeleteTextures = LoadFunction<Gles.glDeleteTextures_t>("glDeleteTextures");
                Gles._glBindTexture = LoadFunction<Gles.glBindTexture_t>("glBindTexture");
                Gles._glActiveTexture = LoadFunction<Gles.glActiveTexture_t>("glActiveTexture");
                Gles._glTexParameteri = LoadFunction<Gles.glTexParameteri_t>("glTexParameteri");
                Gles._glTexImage2D = LoadFunction<Gles.glTexImage2D_t>("glTexImage2D");
                Gles._glPixelStorei = LoadFunction<Gles.glPixelStorei_t>("glPixelStorei");
                Gles._glGenerateMipmap = LoadFunction<Gles.glGenerateMipmap_t>("glGenerateMipmap");
                Gles._glCullFace = LoadFunction<Gles.glCullFace_t>("glCullFace");
                Gles._glGenFramebuffers = LoadFunction<Gles.glGenFramebuffers_t>("glGenFramebuffers");
                Gles._glDeleteFramebuffers = LoadFunction<Gles.glDeleteFramebuffers_t>("glDeleteFramebuffers");
                Gles._glBindFramebuffer = LoadFunction<Gles.glBindFramebuffer_t>("glBindFramebuffer");
                Gles._glFramebufferTexture2D = LoadFunction<Gles.glFramebufferTexture2D_t>("glFramebufferTexture2D");
                Gles._glFramebufferRenderbuffer = LoadFunction<Gles.glFramebufferRenderbuffer_t>("glFramebufferRenderbuffer");
                Gles._glCheckFramebufferStatus = LoadFunction<Gles.glCheckFramebufferStatus_t>("glCheckFramebufferStatus");
                Gles._glGenRenderbuffers = LoadFunction<Gles.glGenRenderbuffers_t>("glGenRenderbuffers");
                Gles._glDeleteRenderbuffers = LoadFunction<Gles.glDeleteRenderbuffers_t>("glDeleteRenderbuffers");
                Gles._glBindRenderbuffer = LoadFunction<Gles.glBindRenderbuffer_t>("glBindRenderbuffer");
                Gles._glRenderbufferStorage = LoadFunction<Gles.glRenderbufferStorage_t>("glRenderbufferStorage");
                Gles._glBlitFramebuffer = TryLoadFunction<Gles.glBlitFramebuffer_t>("glBlitFramebuffer");
                Gles._glGenSamplers = TryLoadFunction<Gles.glGenSamplers_t>("glGenSamplers");
                Gles._glDeleteSamplers = TryLoadFunction<Gles.glDeleteSamplers_t>("glDeleteSamplers");
                Gles._glBindSampler = TryLoadFunction<Gles.glBindSampler_t>("glBindSampler");
                Gles._glSamplerParameteri = TryLoadFunction<Gles.glSamplerParameteri_t>("glSamplerParameteri");
                Gles._glSamplerParameterf = TryLoadFunction<Gles.glSamplerParameterf_t>("glSamplerParameterf");
                Gles._glGenQueries = TryLoadFunction<Gles.glGenQueries_t>("glGenQueries");
                Gles._glDeleteQueries = TryLoadFunction<Gles.glDeleteQueries_t>("glDeleteQueries");
                Gles._glBeginQuery = TryLoadFunction<Gles.glBeginQuery_t>("glBeginQuery");
                Gles._glEndQuery = TryLoadFunction<Gles.glEndQuery_t>("glEndQuery");
                Gles._glGetQueryObjectuiv = TryLoadFunction<Gles.glGetQueryObjectuiv_t>("glGetQueryObjectuiv");
                Gles._glGetQueryObjectui64v = TryLoadFunction<Gles.glGetQueryObjectui64v_t>("glGetQueryObjectui64v");
                Gles._glFenceSync = TryLoadFunction<Gles.glFenceSync_t>("glFenceSync");
                Gles._glDeleteSync = TryLoadFunction<Gles.glDeleteSync_t>("glDeleteSync");
                Gles._glClientWaitSync = TryLoadFunction<Gles.glClientWaitSync_t>("glClientWaitSync");
                Gles._glWaitSync = TryLoadFunction<Gles.glWaitSync_t>("glWaitSync");
                Gles._glIsSync = TryLoadFunction<Gles.glIsSync_t>("glIsSync");
                Gles._glGetSynciv = TryLoadFunction<Gles.glGetSynciv_t>("glGetSynciv");
                Gles._glGenTransformFeedbacks = TryLoadFunction<Gles.glGenTransformFeedbacks_t>("glGenTransformFeedbacks");
                Gles._glDeleteTransformFeedbacks = TryLoadFunction<Gles.glDeleteTransformFeedbacks_t>("glDeleteTransformFeedbacks");
                Gles._glBindTransformFeedback = TryLoadFunction<Gles.glBindTransformFeedback_t>("glBindTransformFeedback");
                Gles._glBeginTransformFeedback = TryLoadFunction<Gles.glBeginTransformFeedback_t>("glBeginTransformFeedback");
                Gles._glEndTransformFeedback = TryLoadFunction<Gles.glEndTransformFeedback_t>("glEndTransformFeedback");
                Gles._glPauseTransformFeedback = TryLoadFunction<Gles.glPauseTransformFeedback_t>("glPauseTransformFeedback");
                Gles._glResumeTransformFeedback = TryLoadFunction<Gles.glResumeTransformFeedback_t>("glResumeTransformFeedback");
                Gles._glTransformFeedbackVaryings = TryLoadFunction<Gles.glTransformFeedbackVaryings_t>("glTransformFeedbackVaryings");
                Gles._glGetTransformFeedbackVarying = TryLoadFunction<Gles.glGetTransformFeedbackVarying_t>("glGetTransformFeedbackVarying");
                Gles._glBindBufferBase = TryLoadFunction<Gles.glBindBufferBase_t>("glBindBufferBase");
                Gles._glBindBufferRange = TryLoadFunction<Gles.glBindBufferRange_t>("glBindBufferRange");

                Gles._glDebugMessageCallback = TryLoadFunction<Gles.glDebugMessageCallback_t>("glDebugMessageCallback");
                if (Gles._glDebugMessageCallback == null)
                    Gles._glDebugMessageCallback = TryLoadFunction<Gles.glDebugMessageCallback_t>("glDebugMessageCallbackKHR");

                Gles._glDebugMessageControl = TryLoadFunction<Gles.glDebugMessageControl_t>("glDebugMessageControl");
                if (Gles._glDebugMessageControl == null)
                    Gles._glDebugMessageControl = TryLoadFunction<Gles.glDebugMessageControl_t>("glDebugMessageControlKHR");

                Volatile.Write(ref _loaded, 1);
            }
        }

        private static T LoadFunction<T>(string name) where T : Delegate
        {
            var p = global::AngleBridgeNative.AB_GetProcAddress(name);
            if (p == IntPtr.Zero) throw new EntryPointNotFoundException(string.Format("AB_GetProcAddress(\"{0}\") returned NULL.", name));
            return Marshal.GetDelegateForFunctionPointer<T>(p);
        }

        private static T? TryLoadFunction<T>(string name) where T : Delegate
        {
            var p = global::AngleBridgeNative.AB_GetProcAddress(name);
            return p == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<T>(p);
        }
    }
}
