using AngleBridge.Managed.GLES;
using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    partial class AngleGl
    {
        public readonly struct ProgramsApi
        {
            private readonly AngleGl _gl;

            internal ProgramsApi(AngleGl gl)
            {
                _gl = gl;
            }

            public GlProgramHandle CreateProgram()
            {
                _gl.Guard();
                return new GlProgramHandle(I32(Gles.CreateProgram()));
            }

            public AngleProgram CreateProgramObject()
            {
                _gl.Guard();
                var handle = new GlProgramHandle(I32(Gles.CreateProgram()));
                return new AngleProgram(_gl, handle);
            }

            public void AttachShader(GlProgramHandle program, GlShaderHandle shader)
            {
                _gl.Guard();
                Gles.AttachShader(U32(program.Value), U32(shader.Value));
            }

            public void AttachShader(AngleProgram program, AngleShader shader)
            {
                _gl.Guard();
                _gl.ValidateProgram(program);
                _gl.ValidateShader(shader);
                Gles.AttachShader(U32(program.Handle.Value), U32(shader.Handle.Value));
            }

            public void LinkProgram(GlProgramHandle program)
            {
                _gl.Guard();
                Gles.LinkProgram(U32(program.Value));
            }

            public void LinkProgram(AngleProgram program)
            {
                _gl.Guard();
                _gl.ValidateProgram(program);
                Gles.LinkProgram(U32(program.Handle.Value));
            }

            public int GetProgramInt(GlProgramHandle program, ProgramParameter pname)
            {
                _gl.Guard();
                return Gles.GetProgramInt(U32(program.Value), U32((int)pname));
            }

            public int GetProgramInt(AngleProgram program, ProgramParameter pname)
            {
                _gl.Guard();
                _gl.ValidateProgram(program);
                return Gles.GetProgramInt(U32(program.Handle.Value), U32((int)pname));
            }

            public string GetProgramInfoLog(GlProgramHandle program)
            {
                _gl.Guard();
                return Gles.GetProgramInfoLog(U32(program.Value));
            }

            public string GetProgramInfoLog(AngleProgram program)
            {
                _gl.Guard();
                _gl.ValidateProgram(program);
                return Gles.GetProgramInfoLog(U32(program.Handle.Value));
            }

            public void UseProgram(GlProgramHandle program)
            {
                _gl.Guard();
                Gles.UseProgram(U32(program.Value));
            }

            public void UseProgram(AngleProgram program)
            {
                _gl.Guard();
                _gl.ValidateProgram(program);
                Gles.UseProgram(U32(program.Handle.Value));
            }

            public void DeleteProgram(GlProgramHandle program)
            {
                _gl.Guard();
                if (program.IsNull) return;
                Gles.DeleteProgram(U32(program.Value));
            }

            public int GetAttribLocation(GlProgramHandle program, string name)
            {
                _gl.Guard();
                return Gles.GetAttribLocation(U32(program.Value), name);
            }

            public int GetAttribLocation(AngleProgram program, string name)
            {
                _gl.Guard();
                _gl.ValidateProgram(program);
                return Gles.GetAttribLocation(U32(program.Handle.Value), name);
            }

            public int GetUniformLocation(GlProgramHandle program, string name)
            {
                _gl.Guard();
                return Gles.GetUniformLocation(U32(program.Value), name);
            }

            public int GetUniformLocation(AngleProgram program, string name)
            {
                _gl.Guard();
                _gl.ValidateProgram(program);
                return Gles.GetUniformLocation(U32(program.Handle.Value), name);
            }

            public void Uniform1i(int location, int v0)
            {
                _gl.Guard();
                Gles.Uniform1i(location, v0);
            }

            public void Uniform1f(int location, float v0)
            {
                _gl.Guard();
                Gles.Uniform1f(location, v0);
            }

            public void Uniform3f(int location, float v0, float v1, float v2)
            {
                _gl.Guard();
                Gles.Uniform3f(location, v0, v1, v2);
            }

            public void Uniform4f(int location, float v0, float v1, float v2, float v3)
            {
                _gl.Guard();
                Gles.Uniform4f(location, v0, v1, v2, v3);
            }

            public void UniformMatrix4fv(int location, bool transpose, float[] value)
            {
                _gl.Guard();
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (value.Length < 16)
                    throw new ArgumentException("Matrix4fv requires at least 16 floats.", nameof(value));
                Gles.UniformMatrix4fv(location, transpose, value);
            }
        }
    }
}
