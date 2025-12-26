using System;
using System.Threading;

namespace AngleBridge.Managed.Runtime
{
    internal sealed class AngleContext : IDisposable
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
            return CreateOffscreen(glesMajor, width, height, backend: 0, deviceType: 0, swapInterval: -1);
        }

        public static AngleContext CreateOffscreen(int glesMajor, int width, int height, int backend, int deviceType, int swapInterval)
        {
            var h = global::AngleBridgeNative.AB_CreateOffscreenContextEx(glesMajor, width, height, backend, deviceType, swapInterval);
            if (h == IntPtr.Zero) throw new InvalidOperationException("AB_CreateOffscreenContextEx failed.");
            return new AngleContext(h, ownsHandle: true);
        }

        public static AngleContext CreateWindow(IntPtr nativeWindow, int glesMajor)
        {
            return CreateWindow(nativeWindow, glesMajor, backend: 0, deviceType: 0, swapInterval: -1);
        }

        public static AngleContext CreateWindow(IntPtr nativeWindow, int glesMajor, int backend, int deviceType, int swapInterval)
        {
            var h = global::AngleBridgeNative.AB_CreateWindowContextEx(nativeWindow, glesMajor, backend, deviceType, swapInterval);
            if (h == IntPtr.Zero) throw new InvalidOperationException("AB_CreateWindowContextEx failed.");
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
}
