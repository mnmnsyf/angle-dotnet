using System;
using System.Runtime.InteropServices;

class Program
{
    const int GL_VERSION = 0x1F02;

    static void Main()
    {
        var h = AngleBridgeNative.AB_CreateOffscreenContext(3, 16, 16);
        if (h == IntPtr.Zero) throw new Exception("Create context failed");

        var p = AngleBridgeNative.AB_GetGLString(GL_VERSION);
        Console.WriteLine("GL_VERSION: " + Marshal.PtrToStringAnsi(p));

        AngleBridgeNative.AB_DestroyContext(h);
    }
}
