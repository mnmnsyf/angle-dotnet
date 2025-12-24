using System;
using System.Runtime.InteropServices;

internal static class AngleBridgeNative
{
    [DllImport("AngleBridge", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr AB_CreateOffscreenContext(int glesMajor, int width, int height);

    [DllImport("AngleBridge", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void AB_DestroyContext(IntPtr h);

    [DllImport("AngleBridge", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr AB_GetGLString(int name);


    [DllImport("AngleBridge", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr AB_CreateWindowContext(IntPtr nativeWindow, int glesMajor);

    [DllImport("AngleBridge", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int AB_SwapBuffers(IntPtr ctx);

    [DllImport("AngleBridge", CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr AB_GetProcAddress([MarshalAs(UnmanagedType.LPStr)] string name);

    [DllImport("AngleBridge", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void AB_NoOp();

    [DllImport("AngleBridge", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void AB_NoOpN(int n);

    [DllImport("AngleBridge", CallingConvention = CallingConvention.Cdecl)]
    internal static extern void AB_GlCallBatch(int n);
}
