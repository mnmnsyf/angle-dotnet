using AngleBridge.Managed.GLES;
using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    partial class AngleGl
    {
        public readonly struct BuffersApi
        {
            private readonly AngleGl _gl;

            internal BuffersApi(AngleGl gl)
            {
                _gl = gl;
            }

            public GlBufferHandle GenBuffer()
            {
                _gl.Guard();
                return new GlBufferHandle(I32(Gles.GenBuffer()));
            }

            public AngleBuffer CreateBufferObject()
            {
                _gl.Guard();
                var handle = new GlBufferHandle(I32(Gles.GenBuffer()));
                return new AngleBuffer(_gl, handle);
            }

            public void DeleteBuffer(GlBufferHandle buffer)
            {
                _gl.Guard();
                if (buffer.IsNull) return;
                Gles.DeleteBuffer(U32(buffer.Value));
            }

            public void BindBuffer(BufferTarget target, GlBufferHandle buffer)
            {
                _gl.Guard();
                Gles.BindBuffer(U32((int)target), U32(buffer.Value));
            }

            public void BindBuffer(BufferTarget target, AngleBuffer buffer)
            {
                _gl.Guard();
                _gl.ValidateBuffer(buffer);
                Gles.BindBuffer(U32((int)target), U32(buffer.Handle.Value));
            }

            public void BufferData(BufferTarget target, float[] data, BufferUsage usage)
            {
                _gl.Guard();
                Gles.BufferData(U32((int)target), data, U32((int)usage));
            }

            [CLSCompliant(false)]
            public void BufferData(BufferTarget target, ushort[] data, BufferUsage usage)
            {
                _gl.Guard();
                Gles.BufferData(U32((int)target), data, U32((int)usage));
            }

            [CLSCompliant(false)]
            public void BindBufferBase(BufferTarget target, uint index, GlBufferHandle buffer)
            {
                _gl.Guard();
                Gles.BindBufferBase(U32((int)target), index, U32(buffer.Value));
            }

            [CLSCompliant(false)]
            public void BindBufferBase(BufferTarget target, uint index, AngleBuffer buffer)
            {
                _gl.Guard();
                _gl.ValidateBuffer(buffer);
                Gles.BindBufferBase(U32((int)target), index, U32(buffer.Handle.Value));
            }

            [CLSCompliant(false)]
            public void BindBufferRange(BufferTarget target, uint index, GlBufferHandle buffer, nint offset, nint size)
            {
                _gl.Guard();
                Gles.BindBufferRange(U32((int)target), index, U32(buffer.Value), offset, size);
            }

            [CLSCompliant(false)]
            public void BindBufferRange(BufferTarget target, uint index, AngleBuffer buffer, nint offset, nint size)
            {
                _gl.Guard();
                _gl.ValidateBuffer(buffer);
                Gles.BindBufferRange(U32((int)target), index, U32(buffer.Handle.Value), offset, size);
            }
        }
    }
}
