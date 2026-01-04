using AngleBridge.Managed.GLES;
using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    partial class AngleGl
    {
        public readonly struct StateApi
        {
            private readonly AngleGl _gl;

            internal StateApi(AngleGl gl)
            {
                _gl = gl;
            }

            public void Viewport(int x, int y, int width, int height)
            {
                _gl.Guard();
                Gles.Viewport(x, y, width, height);
            }

            public void ClearColor(float r, float g, float b, float a)
            {
                _gl.Guard();
                Gles.ClearColor(r, g, b, a);
            }

            public void Clear(ClearBufferMask mask)
            {
                _gl.Guard();
                Gles.Clear(U32((int)mask));
            }

            public void ClearStencil(int s)
            {
                _gl.Guard();
                Gles.ClearStencil(s);
            }

            public void ClearDepth(float depth)
            {
                _gl.Guard();
                Gles.ClearDepthf(depth);
            }

            public void Enable(EnableCap cap)
            {
                _gl.Guard();
                Gles.Enable(U32((int)cap));
            }

            public void Disable(EnableCap cap)
            {
                _gl.Guard();
                Gles.Disable(U32((int)cap));
            }

            public void BlendFunc(BlendFactor sfactor, BlendFactor dfactor)
            {
                _gl.Guard();
                Gles.BlendFunc(U32((int)sfactor), U32((int)dfactor));
            }

            public void StencilFunc(StencilFunction func, int reference, int mask)
            {
                _gl.Guard();
                Gles.StencilFunc(U32((int)func), reference, U32(mask));
            }

            public void StencilOp(StencilOp sfail, StencilOp dpfail, StencilOp dppass)
            {
                _gl.Guard();
                Gles.StencilOp(U32((int)sfail), U32((int)dpfail), U32((int)dppass));
            }

            public void StencilMask(int mask)
            {
                _gl.Guard();
                Gles.StencilMask(U32(mask));
            }

            public void CullFace(CullFaceMode mode)
            {
                _gl.Guard();
                Gles.CullFace(U32((int)mode));
            }
        }
    }
}
