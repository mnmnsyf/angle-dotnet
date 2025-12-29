using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    public sealed class AngleTransformFeedback : IDisposable
    {
        private readonly AngleGl _gl;
        private GlTransformFeedbackHandle _handle;
        private bool _disposed;

        internal AngleTransformFeedback(AngleGl gl, GlTransformFeedbackHandle handle)
        {
            _gl = gl ?? throw new ArgumentNullException(nameof(gl));
            _handle = handle;
        }

        internal AngleDevice Device { get { return _gl.Device; } }
        internal GlTransformFeedbackHandle Handle { get { return _handle; } }
        public bool IsDisposed { get { return _disposed; } }

        internal void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AngleTransformFeedback));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_gl.Device.IsDisposed)
            {
                _handle = new GlTransformFeedbackHandle(0);
                return;
            }

            _gl.DeleteTransformFeedback(_handle);
            _handle = new GlTransformFeedbackHandle(0);
        }
    }
}
