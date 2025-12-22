using System;
using System.Threading;

namespace AngleBridge.Managed.Runtime;

public sealed class AngleContext : IDisposable
{
    private IntPtr _handle;
    private readonly bool _ownsHandle;

    public AngleContext(IntPtr handle, bool ownsHandle = true)
    {
        if (handle == IntPtr.Zero) throw new ArgumentException("Handle must be non-zero.", nameof(handle));
        _handle = handle;
        _ownsHandle = ownsHandle;
    }

    public IntPtr Handle => _handle;

    public static AngleContext CreateOffscreen(int glesMajor, int width, int height)
    {
        var h = global::AngleBridgeNative.AB_CreateOffscreenContext(glesMajor, width, height);
        if (h == IntPtr.Zero) throw new InvalidOperationException("AB_CreateOffscreenContext failed.");
        return new AngleContext(h, ownsHandle: true);
    }

    public static AngleContext CreateWindow(IntPtr nativeWindow, int glesMajor)
    {
        var h = global::AngleBridgeNative.AB_CreateWindowContext(nativeWindow, glesMajor);
        if (h == IntPtr.Zero) throw new InvalidOperationException("AB_CreateWindowContext failed.");
        return new AngleContext(h, ownsHandle: true);
    }

    public void Swap()
    {
        var h = _handle;
        if (h == IntPtr.Zero) throw new ObjectDisposedException(nameof(AngleContext));
        if (global::AngleBridgeNative.AB_SwapBuffers(h) == 0)
            throw new InvalidOperationException("AB_SwapBuffers failed.");
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        var h = Interlocked.Exchange(ref _handle, IntPtr.Zero);
        if (h == IntPtr.Zero) return;
        if (_ownsHandle) global::AngleBridgeNative.AB_DestroyContext(h);
    }

    ~AngleContext()
    {
        Dispose(disposing: false);
    }
}
