using AngleBridge.Sdk.Interop;

namespace AngleBridge.Sdk.Runtime
{
    public sealed class AngleTexture : System.IDisposable
    {
        private readonly AngleGl _gl;
        private GlTextureHandle _handle;

        internal AngleTexture(AngleGl gl, GlTextureHandle handle)
        {
            _gl = gl ?? throw new System.ArgumentNullException(nameof(gl));
            _handle = handle;
        }

        internal AngleDevice Device { get { return _gl.Device; } }
        internal GlTextureHandle Handle { get { return _handle; } }

        public void Dispose()
        {
            var h = _handle;
            if (h.IsNull) return;
            _handle = default;
            _gl.DeleteTexture(h);
        }

        internal void ThrowIfDisposed()
        {
            if (_handle.IsNull) throw new System.ObjectDisposedException(nameof(AngleTexture));
        }
    }
}
