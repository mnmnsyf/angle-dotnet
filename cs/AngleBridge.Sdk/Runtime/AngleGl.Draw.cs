using AngleBridge.Managed.GLES;
using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    partial class AngleGl
    {
        public readonly struct DrawApi
        {
            private readonly AngleGl _gl;

            internal DrawApi(AngleGl gl)
            {
                _gl = gl;
            }

            public bool IsInstancedArraysAvailable
            {
                get
                {
                    _gl.Guard();
                    return Gles.IsInstancedArraysAvailable;
                }
            }

            public void DrawArrays(PrimitiveType mode, int first, int count)
            {
                _gl.Guard();
                Gles.DrawArrays(U32((int)mode), first, count);
            }

            public void DrawElements(PrimitiveType mode, int count, IndexElementType type, int offsetBytes)
            {
                _gl.Guard();
                Gles.DrawElements(U32((int)mode), count, U32((int)type), offsetBytes);
            }

            public void Finish()
            {
                _gl.Guard();
                Gles.Finish();
            }

            public void DrawArraysInstanced(PrimitiveType mode, int first, int count, int instanceCount)
            {
                _gl.Guard();
                if (instanceCount <= 0) throw new ArgumentOutOfRangeException(nameof(instanceCount), "Instance count must be positive.");
                Gles.DrawArraysInstanced(U32((int)mode), first, count, instanceCount);
            }

            public void DrawElementsInstanced(PrimitiveType mode, int count, IndexElementType type, int offsetBytes, int instanceCount)
            {
                _gl.Guard();
                if (instanceCount <= 0) throw new ArgumentOutOfRangeException(nameof(instanceCount), "Instance count must be positive.");
                Gles.DrawElementsInstanced(U32((int)mode), count, U32((int)type), offsetBytes, instanceCount);
            }
        }
    }
}
