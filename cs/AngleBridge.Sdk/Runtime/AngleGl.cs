using AngleBridge.Managed.GLES;
using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    public sealed class AngleGl
    {
        private readonly AngleDevice _device;

        internal AngleGl(AngleDevice device)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));
        }

        internal AngleDevice Device { get { return _device; } }

        private void Guard()
        {
            _device.VerifyAccess();
            _device.ThrowIfDisposed();
        }

        private static uint U32(int v) { return unchecked((uint)v); }
        private static int I32(uint v) { return unchecked((int)v); }

        public void Viewport(int x, int y, int width, int height)
        {
            Guard();
            Gles.Viewport(x, y, width, height);
        }

        public void ClearColor(float r, float g, float b, float a)
        {
            Guard();
            Gles.ClearColor(r, g, b, a);
        }

        public void Clear(ClearBufferMask mask)
        {
            Guard();
            Gles.Clear(U32((int)mask));
        }

        public GlShaderHandle CreateShader(ShaderType type)
        {
            Guard();
            return new GlShaderHandle(I32(Gles.CreateShader(U32((int)type))));
        }

        public AngleShader CreateShaderObject(ShaderType type)
        {
            Guard();
            var handle = new GlShaderHandle(I32(Gles.CreateShader(U32((int)type))));
            return new AngleShader(this, handle);
        }

        public void ShaderSource(GlShaderHandle shader, string source)
        {
            Guard();
            Gles.ShaderSource(U32(shader.Value), source);
        }

        public void ShaderSource(AngleShader shader, string source)
        {
            Guard();
            ValidateShader(shader);
            Gles.ShaderSource(U32(shader.Handle.Value), source);
        }

        public void CompileShader(GlShaderHandle shader)
        {
            Guard();
            Gles.CompileShader(U32(shader.Value));
        }

        public void CompileShader(AngleShader shader)
        {
            Guard();
            ValidateShader(shader);
            Gles.CompileShader(U32(shader.Handle.Value));
        }

        public int GetShaderInt(GlShaderHandle shader, ShaderParameter pname)
        {
            Guard();
            return Gles.GetShaderInt(U32(shader.Value), U32((int)pname));
        }

        public int GetShaderInt(AngleShader shader, ShaderParameter pname)
        {
            Guard();
            ValidateShader(shader);
            return Gles.GetShaderInt(U32(shader.Handle.Value), U32((int)pname));
        }

        public string GetShaderInfoLog(GlShaderHandle shader)
        {
            Guard();
            return Gles.GetShaderInfoLog(U32(shader.Value));
        }

        public string GetShaderInfoLog(AngleShader shader)
        {
            Guard();
            ValidateShader(shader);
            return Gles.GetShaderInfoLog(U32(shader.Handle.Value));
        }

        public void DeleteShader(GlShaderHandle shader)
        {
            Guard();
            if (shader.IsNull) return;
            Gles.DeleteShader(U32(shader.Value));
        }

        public GlProgramHandle CreateProgram()
        {
            Guard();
            return new GlProgramHandle(I32(Gles.CreateProgram()));
        }

        public AngleProgram CreateProgramObject()
        {
            Guard();
            var handle = new GlProgramHandle(I32(Gles.CreateProgram()));
            return new AngleProgram(this, handle);
        }

        public void AttachShader(GlProgramHandle program, GlShaderHandle shader)
        {
            Guard();
            Gles.AttachShader(U32(program.Value), U32(shader.Value));
        }

        public void AttachShader(AngleProgram program, AngleShader shader)
        {
            Guard();
            ValidateProgram(program);
            ValidateShader(shader);
            Gles.AttachShader(U32(program.Handle.Value), U32(shader.Handle.Value));
        }

        public void LinkProgram(GlProgramHandle program)
        {
            Guard();
            Gles.LinkProgram(U32(program.Value));
        }

        public void LinkProgram(AngleProgram program)
        {
            Guard();
            ValidateProgram(program);
            Gles.LinkProgram(U32(program.Handle.Value));
        }

        public int GetProgramInt(GlProgramHandle program, ProgramParameter pname)
        {
            Guard();
            return Gles.GetProgramInt(U32(program.Value), U32((int)pname));
        }

        public int GetProgramInt(AngleProgram program, ProgramParameter pname)
        {
            Guard();
            ValidateProgram(program);
            return Gles.GetProgramInt(U32(program.Handle.Value), U32((int)pname));
        }

        public string GetProgramInfoLog(GlProgramHandle program)
        {
            Guard();
            return Gles.GetProgramInfoLog(U32(program.Value));
        }

        public string GetProgramInfoLog(AngleProgram program)
        {
            Guard();
            ValidateProgram(program);
            return Gles.GetProgramInfoLog(U32(program.Handle.Value));
        }

        public void UseProgram(GlProgramHandle program)
        {
            Guard();
            Gles.UseProgram(U32(program.Value));
        }

        public void UseProgram(AngleProgram program)
        {
            Guard();
            ValidateProgram(program);
            Gles.UseProgram(U32(program.Handle.Value));
        }

        public void DeleteProgram(GlProgramHandle program)
        {
            Guard();
            if (program.IsNull) return;
            Gles.DeleteProgram(U32(program.Value));
        }

        public int GetAttribLocation(GlProgramHandle program, string name)
        {
            Guard();
            return Gles.GetAttribLocation(U32(program.Value), name);
        }

        public int GetAttribLocation(AngleProgram program, string name)
        {
            Guard();
            ValidateProgram(program);
            return Gles.GetAttribLocation(U32(program.Handle.Value), name);
        }

        public int GetUniformLocation(GlProgramHandle program, string name)
        {
            Guard();
            return Gles.GetUniformLocation(U32(program.Value), name);
        }

        public int GetUniformLocation(AngleProgram program, string name)
        {
            Guard();
            ValidateProgram(program);
            return Gles.GetUniformLocation(U32(program.Handle.Value), name);
        }

        public void Uniform4f(int location, float v0, float v1, float v2, float v3)
        {
            Guard();
            Gles.Uniform4f(location, v0, v1, v2, v3);
        }

        public GlBufferHandle GenBuffer()
        {
            Guard();
            return new GlBufferHandle(I32(Gles.GenBuffer()));
        }

        public AngleBuffer CreateBufferObject()
        {
            Guard();
            var handle = new GlBufferHandle(I32(Gles.GenBuffer()));
            return new AngleBuffer(this, handle);
        }

        public void DeleteBuffer(GlBufferHandle buffer)
        {
            Guard();
            if (buffer.IsNull) return;
            Gles.DeleteBuffer(U32(buffer.Value));
        }

        public void BindBuffer(BufferTarget target, GlBufferHandle buffer)
        {
            Guard();
            Gles.BindBuffer(U32((int)target), U32(buffer.Value));
        }

        public void BindBuffer(BufferTarget target, AngleBuffer buffer)
        {
            Guard();
            ValidateBuffer(buffer);
            Gles.BindBuffer(U32((int)target), U32(buffer.Handle.Value));
        }

        public void BufferData(BufferTarget target, float[] data, BufferUsage usage)
        {
            Guard();
            Gles.BufferData(U32((int)target), data, U32((int)usage));
        }

        public void EnableVertexAttribArray(int index)
        {
            Guard();
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "Index must be non-negative.");
            Gles.EnableVertexAttribArray(U32(index));
        }

        public void VertexAttribPointer(int index, int size, VertexAttribType type, bool normalized, int stride, int offsetBytes)
        {
            Guard();
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "Index must be non-negative.");
            Gles.VertexAttribPointer(U32(index), size, U32((int)type), normalized, stride, offsetBytes);
        }

        public void DrawArrays(PrimitiveType mode, int first, int count)
        {
            Guard();
            Gles.DrawArrays(U32((int)mode), first, count);
        }

        public GlVertexArrayHandle GenVertexArray()
        {
            Guard();
            return new GlVertexArrayHandle(I32(Gles.GenVertexArray()));
        }

        public AngleVertexArray CreateVertexArrayObject()
        {
            Guard();
            var handle = new GlVertexArrayHandle(I32(Gles.GenVertexArray()));
            return new AngleVertexArray(this, handle);
        }

        public void BindVertexArray(GlVertexArrayHandle vao)
        {
            Guard();
            Gles.BindVertexArray(U32(vao.Value));
        }

        public void BindVertexArray(AngleVertexArray vao)
        {
            Guard();
            ValidateVertexArray(vao);
            Gles.BindVertexArray(U32(vao.Handle.Value));
        }

        public void DeleteVertexArray(GlVertexArrayHandle vao)
        {
            Guard();
            if (vao.IsNull) return;
            Gles.DeleteVertexArray(U32(vao.Value));
        }

        public void Finish()
        {
            Guard();
            Gles.Finish();
        }

        public bool IsDisjointTimerQueryAvailable
        {
            get
            {
                Guard();
                return Gles.IsDisjointTimerQueryAvailable;
            }
        }

        public int CreateTimerQuery()
        {
            Guard();
            if (!Gles.IsDisjointTimerQueryAvailable)
                throw new NotSupportedException("EXT_disjoint_timer_query not available.");
            return I32(Gles.GenQueryExt());
        }

        public void DeleteTimerQuery(int query)
        {
            Guard();
            if (query == 0) return;
            Gles.DeleteQueryExt(U32(query));
        }

        public void BeginTimerQuery(int query)
        {
            Guard();
            if (query <= 0) throw new ArgumentOutOfRangeException(nameof(query), "Query id must be positive.");
            Gles.BeginQueryExt(Gles.GL_TIME_ELAPSED_EXT, U32(query));
        }

        public void EndTimerQuery()
        {
            Guard();
            Gles.EndQueryExt(Gles.GL_TIME_ELAPSED_EXT);
        }

        public bool TryGetTimerQueryResultAvailable(int query, out bool available)
        {
            Guard();
            if (query <= 0) throw new ArgumentOutOfRangeException(nameof(query), "Query id must be positive.");
            return Gles.TryGetQueryResultAvailableExt(U32(query), out available);
        }

        public bool TryGetTimerQueryResult(int query, out long nanoseconds)
        {
            Guard();
            if (query <= 0) throw new ArgumentOutOfRangeException(nameof(query), "Query id must be positive.");
            if (Gles.TryGetQueryResultExt(U32(query), out var ns))
            {
                nanoseconds = ns > long.MaxValue ? long.MaxValue : (long)ns;
                return true;
            }

            nanoseconds = 0;
            return false;
        }

        public bool TryGetGpuDisjoint(out bool disjoint)
        {
            Guard();
            return Gles.TryGetBoolean(Gles.GL_GPU_DISJOINT_EXT, out disjoint);
        }

        private void ValidateShader(AngleShader shader)
        {
            if (shader == null) throw new ArgumentNullException(nameof(shader));
            if (!ReferenceEquals(shader.Device, _device))
                throw new InvalidOperationException("Shader does not belong to this device.");
            shader.ThrowIfDisposed();
        }

        private void ValidateProgram(AngleProgram program)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            if (!ReferenceEquals(program.Device, _device))
                throw new InvalidOperationException("Program does not belong to this device.");
            program.ThrowIfDisposed();
        }

        private void ValidateBuffer(AngleBuffer buffer)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (!ReferenceEquals(buffer.Device, _device))
                throw new InvalidOperationException("Buffer does not belong to this device.");
            buffer.ThrowIfDisposed();
        }

        private void ValidateVertexArray(AngleVertexArray vao)
        {
            if (vao == null) throw new ArgumentNullException(nameof(vao));
            if (!ReferenceEquals(vao.Device, _device))
                throw new InvalidOperationException("Vertex array does not belong to this device.");
            vao.ThrowIfDisposed();
        }
    }
}
