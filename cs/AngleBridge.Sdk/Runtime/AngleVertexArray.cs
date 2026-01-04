using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    public sealed class AngleVertexArray : IDisposable
    {
        private readonly AngleGl _gl;
        private GlVertexArrayHandle _handle;
        private bool _disposed;

        internal AngleVertexArray(AngleGl gl, GlVertexArrayHandle handle)
        {
            _gl = gl ?? throw new ArgumentNullException(nameof(gl));
            _handle = handle;
        }

        internal AngleDevice Device { get { return _gl.Device; } }
        internal GlVertexArrayHandle Handle { get { return _handle; } }

        public bool IsDisposed { get { return _disposed; } }

        internal void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AngleVertexArray));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_gl.Device.IsDisposed)
            {
                _handle = new GlVertexArrayHandle(0);
                return;
            }

            _gl.VertexArrays.DeleteVertexArray(_handle);
            _handle = new GlVertexArrayHandle(0);
        }
    }
}

