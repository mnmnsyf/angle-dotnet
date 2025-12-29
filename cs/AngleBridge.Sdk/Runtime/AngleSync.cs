using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    public sealed class AngleSync : IDisposable
    {
        private readonly AngleGl _gl;
        private GlSyncHandle _handle;
        private bool _disposed;

        internal AngleSync(AngleGl gl, GlSyncHandle handle)
        {
            _gl = gl ?? throw new ArgumentNullException(nameof(gl));
            _handle = handle;
        }

        internal AngleDevice Device { get { return _gl.Device; } }
        internal GlSyncHandle Handle { get { return _handle; } }
        public bool IsDisposed { get { return _disposed; } }

        internal void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AngleSync));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_gl.Device.IsDisposed)
            {
                _handle = new GlSyncHandle(IntPtr.Zero);
                return;
            }

            _gl.DeleteSync(_handle);
            _handle = new GlSyncHandle(IntPtr.Zero);
        }
    }
}
