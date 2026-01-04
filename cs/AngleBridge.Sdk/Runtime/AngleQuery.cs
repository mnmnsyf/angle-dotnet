using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    public sealed class AngleQuery : IDisposable
    {
        private readonly AngleGl _gl;
        private GlQueryHandle _handle;
        private bool _disposed;

        internal AngleQuery(AngleGl gl, GlQueryHandle handle)
        {
            _gl = gl ?? throw new ArgumentNullException(nameof(gl));
            _handle = handle;
        }

        internal AngleDevice Device { get { return _gl.Device; } }
        internal GlQueryHandle Handle { get { return _handle; } }
        public bool IsDisposed { get { return _disposed; } }

        internal void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AngleQuery));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_gl.Device.IsDisposed)
            {
                _handle = new GlQueryHandle(0);
                return;
            }

            _gl.Queries.DeleteQuery(_handle);
            _handle = new GlQueryHandle(0);
        }
    }
}
