using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    public sealed class AngleBuffer : IDisposable
    {
        private readonly AngleGl _gl;
        private GlBufferHandle _handle;
        private bool _disposed;

        internal AngleBuffer(AngleGl gl, GlBufferHandle handle)
        {
            _gl = gl ?? throw new ArgumentNullException(nameof(gl));
            _handle = handle;
        }

        internal AngleDevice Device { get { return _gl.Device; } }
        internal GlBufferHandle Handle { get { return _handle; } }

        public bool IsDisposed { get { return _disposed; } }

        internal void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AngleBuffer));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_gl.Device.IsDisposed)
            {
                _handle = new GlBufferHandle(0);
                return;
            }

            _gl.Buffers.DeleteBuffer(_handle);
            _handle = new GlBufferHandle(0);
        }
    }
}

