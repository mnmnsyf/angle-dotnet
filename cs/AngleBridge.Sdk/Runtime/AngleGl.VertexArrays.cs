using AngleBridge.Managed.GLES;
using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    partial class AngleGl
    {
        public readonly struct VertexArraysApi
        {
            private readonly AngleGl _gl;

            internal VertexArraysApi(AngleGl gl)
            {
                _gl = gl;
            }

            public GlVertexArrayHandle GenVertexArray()
            {
                _gl.Guard();
                return new GlVertexArrayHandle(I32(Gles.GenVertexArray()));
            }

            public AngleVertexArray CreateVertexArrayObject()
            {
                _gl.Guard();
                var handle = new GlVertexArrayHandle(I32(Gles.GenVertexArray()));
                return new AngleVertexArray(_gl, handle);
            }

            public void BindVertexArray(GlVertexArrayHandle vao)
            {
                _gl.Guard();
                Gles.BindVertexArray(U32(vao.Value));
            }

            public void BindVertexArray(AngleVertexArray vao)
            {
                _gl.Guard();
                _gl.ValidateVertexArray(vao);
                Gles.BindVertexArray(U32(vao.Handle.Value));
            }

            public void DeleteVertexArray(GlVertexArrayHandle vao)
            {
                _gl.Guard();
                if (vao.IsNull) return;
                Gles.DeleteVertexArray(U32(vao.Value));
            }

            public void EnableVertexAttribArray(int index)
            {
                _gl.Guard();
                if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "Index must be non-negative.");
                Gles.EnableVertexAttribArray(U32(index));
            }

            public void VertexAttribPointer(int index, int size, VertexAttribType type, bool normalized, int stride, int offsetBytes)
            {
                _gl.Guard();
                if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "Index must be non-negative.");
                Gles.VertexAttribPointer(U32(index), size, U32((int)type), normalized, stride, offsetBytes);
            }

            public void VertexAttribDivisor(int index, int divisor)
            {
                _gl.Guard();
                if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "Index must be non-negative.");
                if (divisor < 0) throw new ArgumentOutOfRangeException(nameof(divisor), "Divisor must be non-negative.");
                Gles.VertexAttribDivisor(U32(index), U32(divisor));
            }
        }
    }
}
