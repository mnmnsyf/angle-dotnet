using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace AngleBridge.Managed.GLES;

public static class GlesLoader
{
    private const int GL_VERSION = 0x1F02;

    private static int _loaded;
    private static readonly object s_lock = new();

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
            Gles._glUniform4f = LoadFunction<Gles.glUniform4f_t>("glUniform4f");

            Gles._glGenBuffers = LoadFunction<Gles.glGenBuffers_t>("glGenBuffers");
            Gles._glBindBuffer = LoadFunction<Gles.glBindBuffer_t>("glBindBuffer");
            Gles._glBufferData = LoadFunction<Gles.glBufferData_t>("glBufferData");
            Gles._glDeleteBuffers = LoadFunction<Gles.glDeleteBuffers_t>("glDeleteBuffers");

            Gles._glEnableVertexAttribArray = LoadFunction<Gles.glEnableVertexAttribArray_t>("glEnableVertexAttribArray");
            Gles._glVertexAttribPointer = LoadFunction<Gles.glVertexAttribPointer_t>("glVertexAttribPointer");
            Gles._glDrawArrays = LoadFunction<Gles.glDrawArrays_t>("glDrawArrays");

            Gles._glGenVertexArrays = TryLoadFunction<Gles.glGenVertexArrays_t>("glGenVertexArrays");
            Gles._glBindVertexArray = TryLoadFunction<Gles.glBindVertexArray_t>("glBindVertexArray");
            Gles._glDeleteVertexArrays = TryLoadFunction<Gles.glDeleteVertexArrays_t>("glDeleteVertexArrays");

            Gles._glGetError = TryLoadFunction<Gles.glGetError_t>("glGetError");
            Gles._glReadPixels = LoadFunction<Gles.glReadPixels_t>("glReadPixels");
            Gles._glFinish = LoadFunction<Gles.glFinish_t>("glFinish");

            Volatile.Write(ref _loaded, 1);
        }
    }

    private static T LoadFunction<T>(string name) where T : Delegate
    {
        var p = global::AngleBridgeNative.AB_GetProcAddress(name);
        if (p == IntPtr.Zero) throw new EntryPointNotFoundException($"AB_GetProcAddress(\"{name}\") returned NULL.");
        return Marshal.GetDelegateForFunctionPointer<T>(p);
    }

    private static T? TryLoadFunction<T>(string name) where T : Delegate
    {
        var p = global::AngleBridgeNative.AB_GetProcAddress(name);
        return p == IntPtr.Zero ? null : Marshal.GetDelegateForFunctionPointer<T>(p);
    }
}
