using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    public sealed class AngleRenderbuffer : IDisposable
    {
        private readonly AngleGl _gl;
        private GlRenderbufferHandle _handle;
        private bool _disposed;

        internal AngleRenderbuffer(AngleGl gl, GlRenderbufferHandle handle)
        {
            _gl = gl ?? throw new ArgumentNullException(nameof(gl));
            _handle = handle;
        }

        internal AngleDevice Device { get { return _gl.Device; } }
        internal GlRenderbufferHandle Handle { get { return _handle; } }
        public bool IsDisposed { get { return _disposed; } }

        internal void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AngleRenderbuffer));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_gl.Device.IsDisposed)
            {
                _handle = new GlRenderbufferHandle(0);
                return;
            }

            _gl.DeleteRenderbuffer(_handle);
            _handle = new GlRenderbufferHandle(0);
        }
    }
}
