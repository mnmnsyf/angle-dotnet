using AngleBridge.Managed.GLES;
using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    partial class AngleGl
    {
        public readonly struct RenderbuffersApi
        {
            private readonly AngleGl _gl;

            internal RenderbuffersApi(AngleGl gl)
            {
                _gl = gl;
            }

            public GlRenderbufferHandle GenRenderbuffer()
            {
                _gl.Guard();
                return new GlRenderbufferHandle(I32(Gles.GenRenderbuffer()));
            }

            public AngleRenderbuffer CreateRenderbufferObject()
            {
                _gl.Guard();
                var handle = new GlRenderbufferHandle(I32(Gles.GenRenderbuffer()));
                return new AngleRenderbuffer(_gl, handle);
            }

            public void DeleteRenderbuffer(GlRenderbufferHandle renderbuffer)
            {
                _gl.Guard();
                if (renderbuffer.IsNull) return;
                Gles.DeleteRenderbuffer(U32(renderbuffer.Value));
            }

            public void DeleteRenderbuffer(AngleRenderbuffer renderbuffer)
            {
                _gl.Guard();
                _gl.ValidateRenderbuffer(renderbuffer);
                Gles.DeleteRenderbuffer(U32(renderbuffer.Handle.Value));
                renderbuffer.Dispose();
            }

            public void BindRenderbuffer(RenderbufferTarget target, GlRenderbufferHandle renderbuffer)
            {
                _gl.Guard();
                Gles.BindRenderbuffer(U32((int)target), U32(renderbuffer.Value));
            }

            public void BindRenderbuffer(RenderbufferTarget target, AngleRenderbuffer renderbuffer)
            {
                _gl.Guard();
                _gl.ValidateRenderbuffer(renderbuffer);
                Gles.BindRenderbuffer(U32((int)target), U32(renderbuffer.Handle.Value));
            }

            public void RenderbufferStorage(RenderbufferTarget target, RenderbufferInternalFormat internalFormat, int width, int height)
            {
                _gl.Guard();
                if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
                if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
                Gles.RenderbufferStorage(U32((int)target), U32((int)internalFormat), width, height);
            }
        }
    }
}
