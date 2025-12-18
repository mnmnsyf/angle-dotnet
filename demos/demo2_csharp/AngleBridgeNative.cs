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
}