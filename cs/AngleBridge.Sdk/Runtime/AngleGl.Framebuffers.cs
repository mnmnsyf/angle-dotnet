using AngleBridge.Managed.GLES;
using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    partial class AngleGl
    {
        public readonly struct FramebuffersApi
        {
            private readonly AngleGl _gl;

            internal FramebuffersApi(AngleGl gl)
            {
                _gl = gl;
            }

            public bool IsDrawBuffersAvailable
            {
                get
                {
                    _gl.Guard();
                    return Gles.IsDrawBuffersAvailable;
                }
            }

            public bool IsMultiviewAvailable
            {
                get
                {
                    _gl.Guard();
                    return Gles.IsMultiviewAvailable;
                }
            }

            public GlFramebufferHandle GenFramebuffer()
            {
                _gl.Guard();
                return new GlFramebufferHandle(I32(Gles.GenFramebuffer()));
            }

            public AngleFramebuffer CreateFramebufferObject()
            {
                _gl.Guard();
                var handle = new GlFramebufferHandle(I32(Gles.GenFramebuffer()));
                return new AngleFramebuffer(_gl, handle);
            }

            public void DeleteFramebuffer(GlFramebufferHandle framebuffer)
            {
                _gl.Guard();
                if (framebuffer.IsNull) return;
                Gles.DeleteFramebuffer(U32(framebuffer.Value));
            }

            public void DeleteFramebuffer(AngleFramebuffer framebuffer)
            {
                _gl.Guard();
                _gl.ValidateFramebuffer(framebuffer);
                Gles.DeleteFramebuffer(U32(framebuffer.Handle.Value));
                framebuffer.Dispose();
            }

            public void BindFramebuffer(FramebufferTarget target, GlFramebufferHandle framebuffer)
            {
                _gl.Guard();
                Gles.BindFramebuffer(U32((int)target), U32(framebuffer.Value));
            }

            public void BindFramebuffer(FramebufferTarget target, AngleFramebuffer framebuffer)
            {
                _gl.Guard();
                _gl.ValidateFramebuffer(framebuffer);
                Gles.BindFramebuffer(U32((int)target), U32(framebuffer.Handle.Value));
            }

            public void FramebufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, GlTextureHandle texture, int level)
            {
                _gl.Guard();
                Gles.FramebufferTexture2D(U32((int)target), U32((int)attachment), U32((int)textarget), U32(texture.Value), level);
            }

            public void FramebufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, AngleTexture texture, int level)
            {
                _gl.Guard();
                _gl.ValidateTexture(texture);
                Gles.FramebufferTexture2D(U32((int)target), U32((int)attachment), U32((int)textarget), U32(texture.Handle.Value), level);
            }

            public void FramebufferRenderbuffer(FramebufferTarget target, FramebufferAttachment attachment, RenderbufferTarget renderbufferTarget, GlRenderbufferHandle renderbuffer)
            {
                _gl.Guard();
                Gles.FramebufferRenderbuffer(U32((int)target), U32((int)attachment), U32((int)renderbufferTarget), U32(renderbuffer.Value));
            }

            public void FramebufferRenderbuffer(FramebufferTarget target, FramebufferAttachment attachment, RenderbufferTarget renderbufferTarget, AngleRenderbuffer renderbuffer)
            {
                _gl.Guard();
                _gl.ValidateRenderbuffer(renderbuffer);
                Gles.FramebufferRenderbuffer(U32((int)target), U32((int)attachment), U32((int)renderbufferTarget), U32(renderbuffer.Handle.Value));
            }

            public FramebufferStatus CheckFramebufferStatus(FramebufferTarget target)
            {
                _gl.Guard();
                return (FramebufferStatus)I32(Gles.CheckFramebufferStatus(U32((int)target)));
            }

            public void BlitFramebuffer(
                int srcX0, int srcY0, int srcX1, int srcY1,
                int dstX0, int dstY0, int dstX1, int dstY1,
                ClearBufferMask mask, TextureMagFilter filter)
            {
                _gl.Guard();
                Gles.BlitFramebuffer(srcX0, srcY0, srcX1, srcY1, dstX0, dstY0, dstX1, dstY1, U32((int)mask), U32((int)filter));
            }

            public void DrawBuffers(ReadOnlySpan<DrawBuffer> buffers)
            {
                _gl.Guard();
                if (buffers.Length == 0) throw new ArgumentException("buffers must not be empty.", nameof(buffers));
                Span<uint> raw = buffers.Length <= 8 ? stackalloc uint[buffers.Length] : new uint[buffers.Length];
                for (var i = 0; i < buffers.Length; i++)
                    raw[i] = U32((int)buffers[i]);
                Gles.DrawBuffers(raw);
            }

            public void FramebufferTextureMultiview(FramebufferTarget target, FramebufferAttachment attachment, AngleTexture texture, int level, int baseViewIndex, int numViews)
            {
                _gl.Guard();
                _gl.ValidateTexture(texture);
                Gles.FramebufferTextureMultiviewOVR(U32((int)target), U32((int)attachment), U32(texture.Handle.Value), level, baseViewIndex, numViews);
            }
        }
    }
}
