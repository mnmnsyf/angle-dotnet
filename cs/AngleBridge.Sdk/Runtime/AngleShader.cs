using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    public sealed class AngleShader : IDisposable
    {
        private readonly AngleGl _gl;
        private GlShaderHandle _handle;
        private bool _disposed;

        internal AngleShader(AngleGl gl, GlShaderHandle handle)
        {
            _gl = gl ?? throw new ArgumentNullException(nameof(gl));
            _handle = handle;
        }

        internal AngleDevice Device { get { return _gl.Device; } }
        internal GlShaderHandle Handle { get { return _handle; } }

        public bool IsDisposed { get { return _disposed; } }

        internal void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AngleShader));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_gl.Device.IsDisposed)
            {
                _handle = new GlShaderHandle(0);
                return;
            }

            _gl.DeleteShader(_handle);
            _handle = new GlShaderHandle(0);
        }
    }
}

