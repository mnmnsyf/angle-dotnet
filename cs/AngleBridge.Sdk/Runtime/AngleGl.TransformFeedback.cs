using AngleBridge.Managed.GLES;
using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    partial class AngleGl
    {
        public readonly struct TransformFeedbackApi
        {
            private readonly AngleGl _gl;

            internal TransformFeedbackApi(AngleGl gl)
            {
                _gl = gl;
            }

            public AngleTransformFeedback CreateTransformFeedbackObject()
            {
                _gl.Guard();
                var handle = new GlTransformFeedbackHandle(I32(Gles.GenTransformFeedback()));
                return new AngleTransformFeedback(_gl, handle);
            }

            public GlTransformFeedbackHandle GenTransformFeedback()
            {
                _gl.Guard();
                return new GlTransformFeedbackHandle(I32(Gles.GenTransformFeedback()));
            }

            public void DeleteTransformFeedback(GlTransformFeedbackHandle tf)
            {
                _gl.Guard();
                if (tf.IsNull) return;
                Gles.DeleteTransformFeedback(U32(tf.Value));
            }

            public void DeleteTransformFeedback(AngleTransformFeedback tf)
            {
                _gl.Guard();
                _gl.ValidateTransformFeedback(tf);
                Gles.DeleteTransformFeedback(U32(tf.Handle.Value));
                tf.Dispose();
            }

            public void BindTransformFeedback(TransformFeedbackTarget target, GlTransformFeedbackHandle tf)
            {
                _gl.Guard();
                Gles.BindTransformFeedback(U32((int)target), U32(tf.Value));
            }

            public void BindTransformFeedback(TransformFeedbackTarget target, AngleTransformFeedback tf)
            {
                _gl.Guard();
                _gl.ValidateTransformFeedback(tf);
                Gles.BindTransformFeedback(U32((int)target), U32(tf.Handle.Value));
            }

            public void BeginTransformFeedback(PrimitiveType primitiveMode)
            {
                _gl.Guard();
                Gles.BeginTransformFeedback(U32((int)primitiveMode));
            }

            public void EndTransformFeedback()
            {
                _gl.Guard();
                Gles.EndTransformFeedback();
            }

            public void PauseTransformFeedback()
            {
                _gl.Guard();
                Gles.PauseTransformFeedback();
            }

            public void ResumeTransformFeedback()
            {
                _gl.Guard();
                Gles.ResumeTransformFeedback();
            }

            public void TransformFeedbackVaryings(AngleProgram program, string[] varyings, TransformFeedbackBufferMode bufferMode)
            {
                _gl.Guard();
                _gl.ValidateProgram(program);
                if (varyings == null) throw new ArgumentNullException(nameof(varyings));
                Gles.TransformFeedbackVaryings(U32(program.Handle.Value), varyings.Length, varyings, U32((int)bufferMode));
            }

            [CLSCompliant(false)]
            public bool TryGetTransformFeedbackVarying(AngleProgram program, int index, byte[]? nameBufferUtf8, out int length, out int size, out uint type)
            {
                _gl.Guard();
                _gl.ValidateProgram(program);
                var bufSize = nameBufferUtf8?.Length ?? 0;
                return Gles.TryGetTransformFeedbackVarying(U32(program.Handle.Value), U32(index), bufSize, out length, out size, out type, nameBufferUtf8);
            }
        }
    }
}
