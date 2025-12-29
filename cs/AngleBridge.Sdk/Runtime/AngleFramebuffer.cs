using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    public sealed class AngleFramebuffer : IDisposable
    {
        private readonly AngleGl _gl;
        private GlFramebufferHandle _handle;
        private bool _disposed;

        internal AngleFramebuffer(AngleGl gl, GlFramebufferHandle handle)
        {
            _gl = gl ?? throw new ArgumentNullException(nameof(gl));
            _handle = handle;
        }

        internal AngleDevice Device { get { return _gl.Device; } }
        internal GlFramebufferHandle Handle { get { return _handle; } }
        public bool IsDisposed { get { return _disposed; } }

        internal void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AngleFramebuffer));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_gl.Device.IsDisposed)
            {
                _handle = new GlFramebufferHandle(0);
                return;
            }

            _gl.DeleteFramebuffer(_handle);
            _handle = new GlFramebufferHandle(0);
        }
    }
}
