using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    public sealed class AngleSampler : IDisposable
    {
        private readonly AngleGl _gl;
        private GlSamplerHandle _handle;
        private bool _disposed;

        internal AngleSampler(AngleGl gl, GlSamplerHandle handle)
        {
            _gl = gl ?? throw new ArgumentNullException(nameof(gl));
            _handle = handle;
        }

        internal AngleDevice Device { get { return _gl.Device; } }
        internal GlSamplerHandle Handle { get { return _handle; } }
        public bool IsDisposed { get { return _disposed; } }

        internal void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AngleSampler));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_gl.Device.IsDisposed)
            {
                _handle = new GlSamplerHandle(0);
                return;
            }

            _gl.DeleteSampler(_handle);
            _handle = new GlSamplerHandle(0);
        }
    }
}
