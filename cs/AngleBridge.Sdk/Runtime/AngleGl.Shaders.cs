using AngleBridge.Managed.GLES;
using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    partial class AngleGl
    {
        public readonly struct ShadersApi
        {
            private readonly AngleGl _gl;

            internal ShadersApi(AngleGl gl)
            {
                _gl = gl;
            }

            public GlShaderHandle CreateShader(ShaderType type)
            {
                _gl.Guard();
                return new GlShaderHandle(I32(Gles.CreateShader(U32((int)type))));
            }

            public AngleShader CreateShaderObject(ShaderType type)
            {
                _gl.Guard();
                var handle = new GlShaderHandle(I32(Gles.CreateShader(U32((int)type))));
                return new AngleShader(_gl, handle);
            }

            public void ShaderSource(GlShaderHandle shader, string source)
            {
                _gl.Guard();
                Gles.ShaderSource(U32(shader.Value), source);
            }

            public void ShaderSource(AngleShader shader, string source)
            {
                _gl.Guard();
                _gl.ValidateShader(shader);
                Gles.ShaderSource(U32(shader.Handle.Value), source);
            }

            public void CompileShader(GlShaderHandle shader)
            {
                _gl.Guard();
                Gles.CompileShader(U32(shader.Value));
            }

            public void CompileShader(AngleShader shader)
            {
                _gl.Guard();
                _gl.ValidateShader(shader);
                Gles.CompileShader(U32(shader.Handle.Value));
            }

            public int GetShaderInt(GlShaderHandle shader, ShaderParameter pname)
            {
                _gl.Guard();
                return Gles.GetShaderInt(U32(shader.Value), U32((int)pname));
            }

            public int GetShaderInt(AngleShader shader, ShaderParameter pname)
            {
                _gl.Guard();
                _gl.ValidateShader(shader);
                return Gles.GetShaderInt(U32(shader.Handle.Value), U32((int)pname));
            }

            public string GetShaderInfoLog(GlShaderHandle shader)
            {
                _gl.Guard();
                return Gles.GetShaderInfoLog(U32(shader.Value));
            }

            public string GetShaderInfoLog(AngleShader shader)
            {
                _gl.Guard();
                _gl.ValidateShader(shader);
                return Gles.GetShaderInfoLog(U32(shader.Handle.Value));
            }

            public void DeleteShader(GlShaderHandle shader)
            {
                _gl.Guard();
                if (shader.IsNull) return;
                Gles.DeleteShader(U32(shader.Value));
            }
        }
    }
}
