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

        public void ClearStencil(int s)
        {
            Guard();
            Gles.ClearStencil(s);
        }

        public void ClearDepth(float depth)
        {
            Guard();
            Gles.ClearDepthf(depth);
        }

        public void Enable(EnableCap cap)
        {
            Guard();
            Gles.Enable(U32((int)cap));
        }

        public void Disable(EnableCap cap)
        {
            Guard();
            Gles.Disable(U32((int)cap));
        }

        public void BlendFunc(BlendFactor sfactor, BlendFactor dfactor)
        {
            Guard();
            Gles.BlendFunc(U32((int)sfactor), U32((int)dfactor));
        }

        public void StencilFunc(StencilFunction func, int reference, int mask)
        {
            Guard();
            Gles.StencilFunc(U32((int)func), reference, U32(mask));
        }

        public void StencilOp(StencilOp sfail, StencilOp dpfail, StencilOp dppass)
        {
            Guard();
            Gles.StencilOp(U32((int)sfail), U32((int)dpfail), U32((int)dppass));
        }

        public void StencilMask(int mask)
        {
            Guard();
            Gles.StencilMask(U32(mask));
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

        public void Uniform1i(int location, int v0)
        {
            Guard();
            Gles.Uniform1i(location, v0);
        }

        public void Uniform1f(int location, float v0)
        {
            Guard();
            Gles.Uniform1f(location, v0);
        }

        public void Uniform3f(int location, float v0, float v1, float v2)
        {
            Guard();
            Gles.Uniform3f(location, v0, v1, v2);
        }

        public void Uniform4f(int location, float v0, float v1, float v2, float v3)
        {
            Guard();
            Gles.Uniform4f(location, v0, v1, v2, v3);
        }

        public void UniformMatrix4fv(int location, bool transpose, float[] value)
        {
            Guard();
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length < 16)
                throw new ArgumentException("Matrix4fv requires at least 16 floats.", nameof(value));
            Gles.UniformMatrix4fv(location, transpose, value);
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

        [CLSCompliant(false)]
        public void BufferData(BufferTarget target, ushort[] data, BufferUsage usage)
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

        public void DrawElements(PrimitiveType mode, int count, IndexElementType type, int offsetBytes)
        {
            Guard();
            Gles.DrawElements(U32((int)mode), count, U32((int)type), offsetBytes);
        }

        public GlTextureHandle GenTexture()
        {
            Guard();
            return new GlTextureHandle(I32(Gles.GenTexture()));
        }

        public AngleTexture CreateTextureObject()
        {
            Guard();
            var handle = new GlTextureHandle(I32(Gles.GenTexture()));
            return new AngleTexture(this, handle);
        }

        public void DeleteTexture(GlTextureHandle texture)
        {
            Guard();
            if (texture.IsNull) return;
            Gles.DeleteTexture(U32(texture.Value));
        }

        public void BindTexture(TextureTarget target, GlTextureHandle texture)
        {
            Guard();
            Gles.BindTexture(U32((int)target), U32(texture.Value));
        }

        public void BindTexture(TextureTarget target, AngleTexture texture)
        {
            Guard();
            ValidateTexture(texture);
            Gles.BindTexture(U32((int)target), U32(texture.Handle.Value));
        }

        public void ActiveTexture(TextureUnit texture)
        {
            Guard();
            Gles.ActiveTexture(U32((int)texture));
        }

        public void TexParameteri(TextureTarget target, TextureParameterName pname, int param)
        {
            Guard();
            Gles.TexParameteri(U32((int)target), U32((int)pname), param);
        }

        public void TexParameterf(TextureTarget target, TextureParameterName pname, float param)
        {
            Guard();
            Gles.TexParameterf(U32((int)target), U32((int)pname), param);
        }

        public void PixelStore(PixelStoreParameter pname, int param)
        {
            Guard();
            Gles.PixelStorei(U32((int)pname), param);
        }

        public void TexImage2D(TextureTarget target, int level, TextureInternalFormat internalFormat, int width, int height, int border, PixelFormat format, PixelType type, byte[]? pixels)
        {
            Guard();
            Gles.TexImage2D(U32((int)target), level, (int)internalFormat, width, height, border, U32((int)format), U32((int)type), pixels);
        }

        public void CullFace(CullFaceMode mode)
        {
            Guard();
            Gles.CullFace(U32((int)mode));
        }

        public void GenerateMipmap(TextureTarget target)
        {
            Guard();
            Gles.GenerateMipmap(U32((int)target));
        }

        public GlFramebufferHandle GenFramebuffer()
        {
            Guard();
            return new GlFramebufferHandle(I32(Gles.GenFramebuffer()));
        }

        public AngleFramebuffer CreateFramebufferObject()
        {
            Guard();
            var handle = new GlFramebufferHandle(I32(Gles.GenFramebuffer()));
            return new AngleFramebuffer(this, handle);
        }

        public void DeleteFramebuffer(GlFramebufferHandle framebuffer)
        {
            Guard();
            if (framebuffer.IsNull) return;
            Gles.DeleteFramebuffer(U32(framebuffer.Value));
        }

        public void DeleteFramebuffer(AngleFramebuffer framebuffer)
        {
            Guard();
            ValidateFramebuffer(framebuffer);
            Gles.DeleteFramebuffer(U32(framebuffer.Handle.Value));
            framebuffer.Dispose();
        }

        public void BindFramebuffer(FramebufferTarget target, GlFramebufferHandle framebuffer)
        {
            Guard();
            Gles.BindFramebuffer(U32((int)target), U32(framebuffer.Value));
        }

        public void BindFramebuffer(FramebufferTarget target, AngleFramebuffer framebuffer)
        {
            Guard();
            ValidateFramebuffer(framebuffer);
            Gles.BindFramebuffer(U32((int)target), U32(framebuffer.Handle.Value));
        }

        public void FramebufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, GlTextureHandle texture, int level)
        {
            Guard();
            Gles.FramebufferTexture2D(U32((int)target), U32((int)attachment), U32((int)textarget), U32(texture.Value), level);
        }

        public void FramebufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, AngleTexture texture, int level)
        {
            Guard();
            ValidateTexture(texture);
            Gles.FramebufferTexture2D(U32((int)target), U32((int)attachment), U32((int)textarget), U32(texture.Handle.Value), level);
        }

        public void FramebufferRenderbuffer(FramebufferTarget target, FramebufferAttachment attachment, RenderbufferTarget renderbufferTarget, GlRenderbufferHandle renderbuffer)
        {
            Guard();
            Gles.FramebufferRenderbuffer(U32((int)target), U32((int)attachment), U32((int)renderbufferTarget), U32(renderbuffer.Value));
        }

        public void FramebufferRenderbuffer(FramebufferTarget target, FramebufferAttachment attachment, RenderbufferTarget renderbufferTarget, AngleRenderbuffer renderbuffer)
        {
            Guard();
            ValidateRenderbuffer(renderbuffer);
            Gles.FramebufferRenderbuffer(U32((int)target), U32((int)attachment), U32((int)renderbufferTarget), U32(renderbuffer.Handle.Value));
        }

        public FramebufferStatus CheckFramebufferStatus(FramebufferTarget target)
        {
            Guard();
            return (FramebufferStatus)I32(Gles.CheckFramebufferStatus(U32((int)target)));
        }

        public GlRenderbufferHandle GenRenderbuffer()
        {
            Guard();
            return new GlRenderbufferHandle(I32(Gles.GenRenderbuffer()));
        }

        public AngleRenderbuffer CreateRenderbufferObject()
        {
            Guard();
            var handle = new GlRenderbufferHandle(I32(Gles.GenRenderbuffer()));
            return new AngleRenderbuffer(this, handle);
        }

        public void DeleteRenderbuffer(GlRenderbufferHandle renderbuffer)
        {
            Guard();
            if (renderbuffer.IsNull) return;
            Gles.DeleteRenderbuffer(U32(renderbuffer.Value));
        }

        public void DeleteRenderbuffer(AngleRenderbuffer renderbuffer)
        {
            Guard();
            ValidateRenderbuffer(renderbuffer);
            Gles.DeleteRenderbuffer(U32(renderbuffer.Handle.Value));
            renderbuffer.Dispose();
        }

        public void BindRenderbuffer(RenderbufferTarget target, GlRenderbufferHandle renderbuffer)
        {
            Guard();
            Gles.BindRenderbuffer(U32((int)target), U32(renderbuffer.Value));
        }

        public void BindRenderbuffer(RenderbufferTarget target, AngleRenderbuffer renderbuffer)
        {
            Guard();
            ValidateRenderbuffer(renderbuffer);
            Gles.BindRenderbuffer(U32((int)target), U32(renderbuffer.Handle.Value));
        }

        public void RenderbufferStorage(RenderbufferTarget target, RenderbufferInternalFormat internalFormat, int width, int height)
        {
            Guard();
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
            Gles.RenderbufferStorage(U32((int)target), U32((int)internalFormat), width, height);
        }

        public void BlitFramebuffer(
            int srcX0, int srcY0, int srcX1, int srcY1,
            int dstX0, int dstY0, int dstX1, int dstY1,
            ClearBufferMask mask, TextureMagFilter filter)
        {
            Guard();
            Gles.BlitFramebuffer(srcX0, srcY0, srcX1, srcY1, dstX0, dstY0, dstX1, dstY1, U32((int)mask), U32((int)filter));
        }

        public GlSamplerHandle GenSampler()
        {
            Guard();
            return new GlSamplerHandle(I32(Gles.GenSampler()));
        }

        public AngleSampler CreateSamplerObject()
        {
            Guard();
            var handle = new GlSamplerHandle(I32(Gles.GenSampler()));
            return new AngleSampler(this, handle);
        }

        public void DeleteSampler(GlSamplerHandle sampler)
        {
            Guard();
            if (sampler.IsNull) return;
            Gles.DeleteSampler(U32(sampler.Value));
        }

        public void DeleteSampler(AngleSampler sampler)
        {
            Guard();
            ValidateSampler(sampler);
            Gles.DeleteSampler(U32(sampler.Handle.Value));
            sampler.Dispose();
        }

        public void BindSampler(int unit, GlSamplerHandle sampler)
        {
            Guard();
            if (unit < 0) throw new ArgumentOutOfRangeException(nameof(unit));
            Gles.BindSampler(U32(unit), U32(sampler.Value));
        }

        public void BindSampler(int unit, AngleSampler sampler)
        {
            Guard();
            ValidateSampler(sampler);
            if (unit < 0) throw new ArgumentOutOfRangeException(nameof(unit));
            Gles.BindSampler(U32(unit), U32(sampler.Handle.Value));
        }

        public void SamplerParameteri(GlSamplerHandle sampler, TextureParameterName pname, int param)
        {
            Guard();
            Gles.SamplerParameteri(U32(sampler.Value), U32((int)pname), param);
        }

        public void SamplerParameteri(AngleSampler sampler, TextureParameterName pname, int param)
        {
            Guard();
            ValidateSampler(sampler);
            Gles.SamplerParameteri(U32(sampler.Handle.Value), U32((int)pname), param);
        }

        public void SamplerParameterf(GlSamplerHandle sampler, TextureParameterName pname, float param)
        {
            Guard();
            Gles.SamplerParameterf(U32(sampler.Value), U32((int)pname), param);
        }

        public void SamplerParameterf(AngleSampler sampler, TextureParameterName pname, float param)
        {
            Guard();
            ValidateSampler(sampler);
            Gles.SamplerParameterf(U32(sampler.Handle.Value), U32((int)pname), param);
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
                return Gles.IsDisjointTimerQueryAvailable || Gles.TryGetInteger(Gles.GL_QUERY_RESULT_AVAILABLE, out _);
            }
        }

        public AngleQuery CreateQueryObject()
        {
            Guard();
            var handle = new GlQueryHandle(I32(Gles.GenQuery()));
            return new AngleQuery(this, handle);
        }

        public void DeleteQuery(GlQueryHandle query)
        {
            Guard();
            if (query.IsNull) return;
            Gles.DeleteQuery(U32(query.Value));
        }

        public void DeleteQuery(AngleQuery query)
        {
            Guard();
            ValidateQuery(query);
            Gles.DeleteQuery(U32(query.Handle.Value));
            query.Dispose();
        }

        public void BeginQuery(QueryTarget target, AngleQuery query)
        {
            Guard();
            ValidateQuery(query);
            Gles.BeginQuery(U32((int)target), U32(query.Handle.Value));
        }

        public void EndQuery(QueryTarget target)
        {
            Guard();
            Gles.EndQuery(U32((int)target));
        }

        public bool TryGetQueryResultAvailable(AngleQuery query, out bool available)
        {
            Guard();
            ValidateQuery(query);
            if (Gles.TryGetQueryObjectuiv(U32(query.Handle.Value), Gles.GL_QUERY_RESULT_AVAILABLE, out var val))
            {
                available = val != 0;
                return true;
            }
            available = false;
            return false;
        }

        [CLSCompliant(false)]
        public bool TryGetQueryResult(AngleQuery query, out ulong result)
        {
            Guard();
            ValidateQuery(query);
            if (Gles.TryGetQueryObjectui64v(U32(query.Handle.Value), Gles.GL_QUERY_RESULT, out var val))
            {
                result = val;
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryGetGpuDisjoint(out bool disjoint)
        {
            Guard();
            return Gles.TryGetBoolean(Gles.GL_GPU_DISJOINT_EXT, out disjoint);
        }

        public AngleSync FenceSync(SyncCondition condition = SyncCondition.SyncGpuCommandsComplete, SyncFlags flags = SyncFlags.None)
        {
            Guard();
            var sync = Gles.FenceSync(U32((int)condition), U32((int)flags));
            if (sync == IntPtr.Zero)
                throw new InvalidOperationException("glFenceSync returned NULL.");
            return new AngleSync(this, new GlSyncHandle(sync));
        }

        public void DeleteSync(AngleSync sync)
        {
            Guard();
            ValidateSync(sync);
            Gles.DeleteSync(sync.Handle.Value);
            sync.Dispose();
        }

        [CLSCompliant(false)]
        public ClientWaitSyncResult ClientWaitSync(AngleSync sync, SyncFlags flags, ulong timeoutNanoseconds)
        {
            Guard();
            ValidateSync(sync);
            var result = Gles.ClientWaitSync(sync.Handle.Value, U32((int)flags), timeoutNanoseconds);
            return (ClientWaitSyncResult)result;
        }

        [CLSCompliant(false)]
        public void WaitSync(AngleSync sync, SyncFlags flags, ulong timeoutNanoseconds)
        {
            Guard();
            ValidateSync(sync);
            Gles.WaitSync(sync.Handle.Value, U32((int)flags), timeoutNanoseconds);
        }

        [CLSCompliant(false)]
        public void DeleteSync(GlSyncHandle sync)
        {
            Guard();
            if (sync.IsNull) return;
            Gles.DeleteSync(sync.Value);
        }

        public int CreateTimerQuery()
        {
            Guard();
            return I32(Gles.GenQuery());
        }

        public void DeleteTimerQuery(int query)
        {
            Guard();
            if (query == 0) return;
            Gles.DeleteQuery(U32(query));
        }

        public void BeginTimerQuery(int query)
        {
            Guard();
            if (query <= 0) throw new ArgumentOutOfRangeException(nameof(query), "Query id must be positive.");
            Gles.BeginQuery(Gles.GL_TIME_ELAPSED, U32(query));
        }

        public void EndTimerQuery()
        {
            Guard();
            Gles.EndQuery(Gles.GL_TIME_ELAPSED);
        }

        public bool TryGetTimerQueryResultAvailable(int query, out bool available)
        {
            Guard();
            if (query <= 0) throw new ArgumentOutOfRangeException(nameof(query), "Query id must be positive.");
            if (Gles.TryGetQueryObjectuiv(U32(query), Gles.GL_QUERY_RESULT_AVAILABLE, out var val))
            {
                available = val != 0;
                return true;
            }
            available = false;
            return false;
        }

        public bool TryGetTimerQueryResult(int query, out long nanoseconds)
        {
            Guard();
            if (query <= 0) throw new ArgumentOutOfRangeException(nameof(query), "Query id must be positive.");
            if (Gles.TryGetQueryObjectui64v(U32(query), Gles.GL_QUERY_RESULT, out var ns))
            {
                nanoseconds = ns > long.MaxValue ? long.MaxValue : (long)ns;
                return true;
            }

            nanoseconds = 0;
            return false;
        }

        public int GetInteger(GetIntegerParameter pname)
        {
            Guard();
            return Gles.GetInteger(U32((int)pname));
        }

        public bool TryGetInteger(GetIntegerParameter pname, out int value)
        {
            Guard();
            return Gles.TryGetInteger(U32((int)pname), out value);
        }

        public float GetFloat(GetFloatParameter pname)
        {
            Guard();
            return Gles.GetFloat(U32((int)pname));
        }

        public bool TryGetFloat(GetFloatParameter pname, out float value)
        {
            Guard();
            return Gles.TryGetFloat(U32((int)pname), out value);
        }

        public AngleTransformFeedback CreateTransformFeedbackObject()
        {
            Guard();
            var handle = new GlTransformFeedbackHandle(I32(Gles.GenTransformFeedback()));
            return new AngleTransformFeedback(this, handle);
        }

        public GlTransformFeedbackHandle GenTransformFeedback()
        {
            Guard();
            return new GlTransformFeedbackHandle(I32(Gles.GenTransformFeedback()));
        }

        public void DeleteTransformFeedback(GlTransformFeedbackHandle tf)
        {
            Guard();
            if (tf.IsNull) return;
            Gles.DeleteTransformFeedback(U32(tf.Value));
        }

        public void DeleteTransformFeedback(AngleTransformFeedback tf)
        {
            Guard();
            ValidateTransformFeedback(tf);
            Gles.DeleteTransformFeedback(U32(tf.Handle.Value));
            tf.Dispose();
        }

        public void BindTransformFeedback(TransformFeedbackTarget target, GlTransformFeedbackHandle tf)
        {
            Guard();
            Gles.BindTransformFeedback(U32((int)target), U32(tf.Value));
        }

        public void BindTransformFeedback(TransformFeedbackTarget target, AngleTransformFeedback tf)
        {
            Guard();
            ValidateTransformFeedback(tf);
            Gles.BindTransformFeedback(U32((int)target), U32(tf.Handle.Value));
        }

        public void BeginTransformFeedback(PrimitiveType primitiveMode)
        {
            Guard();
            Gles.BeginTransformFeedback(U32((int)primitiveMode));
        }

        public void EndTransformFeedback()
        {
            Guard();
            Gles.EndTransformFeedback();
        }

        public void PauseTransformFeedback()
        {
            Guard();
            Gles.PauseTransformFeedback();
        }

        public void ResumeTransformFeedback()
        {
            Guard();
            Gles.ResumeTransformFeedback();
        }

        public void TransformFeedbackVaryings(AngleProgram program, string[] varyings, TransformFeedbackBufferMode bufferMode)
        {
            Guard();
            ValidateProgram(program);
            if (varyings == null) throw new ArgumentNullException(nameof(varyings));
            Gles.TransformFeedbackVaryings(U32(program.Handle.Value), varyings.Length, varyings, U32((int)bufferMode));
        }

        [CLSCompliant(false)]
        public bool TryGetTransformFeedbackVarying(AngleProgram program, int index, byte[]? nameBufferUtf8, out int length, out int size, out uint type)
        {
            Guard();
            ValidateProgram(program);
            var bufSize = nameBufferUtf8?.Length ?? 0;
            return Gles.TryGetTransformFeedbackVarying(U32(program.Handle.Value), U32(index), bufSize, out length, out size, out type, nameBufferUtf8);
        }

        [CLSCompliant(false)]
        public void BindBufferBase(BufferTarget target, uint index, GlBufferHandle buffer)
        {
            Guard();
            Gles.BindBufferBase(U32((int)target), index, U32(buffer.Value));
        }

        [CLSCompliant(false)]
        public void BindBufferBase(BufferTarget target, uint index, AngleBuffer buffer)
        {
            Guard();
            ValidateBuffer(buffer);
            Gles.BindBufferBase(U32((int)target), index, U32(buffer.Handle.Value));
        }

        [CLSCompliant(false)]
        public void BindBufferRange(BufferTarget target, uint index, GlBufferHandle buffer, nint offset, nint size)
        {
            Guard();
            Gles.BindBufferRange(U32((int)target), index, U32(buffer.Value), offset, size);
        }

        [CLSCompliant(false)]
        public void BindBufferRange(BufferTarget target, uint index, AngleBuffer buffer, nint offset, nint size)
        {
            Guard();
            ValidateBuffer(buffer);
            Gles.BindBufferRange(U32((int)target), index, U32(buffer.Handle.Value), offset, size);
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

        private void ValidateTexture(AngleTexture texture)
        {
            if (texture == null) throw new ArgumentNullException(nameof(texture));
            if (!ReferenceEquals(texture.Device, _device))
                throw new InvalidOperationException("Texture does not belong to this device.");
            texture.ThrowIfDisposed();
        }

        private void ValidateFramebuffer(AngleFramebuffer framebuffer)
        {
            if (framebuffer == null) throw new ArgumentNullException(nameof(framebuffer));
            if (!ReferenceEquals(framebuffer.Device, _device))
                throw new InvalidOperationException("Framebuffer does not belong to this device.");
            framebuffer.ThrowIfDisposed();
        }

        private void ValidateRenderbuffer(AngleRenderbuffer renderbuffer)
        {
            if (renderbuffer == null) throw new ArgumentNullException(nameof(renderbuffer));
            if (!ReferenceEquals(renderbuffer.Device, _device))
                throw new InvalidOperationException("Renderbuffer does not belong to this device.");
            renderbuffer.ThrowIfDisposed();
        }

        private void ValidateSampler(AngleSampler sampler)
        {
            if (sampler == null) throw new ArgumentNullException(nameof(sampler));
            if (!ReferenceEquals(sampler.Device, _device))
                throw new InvalidOperationException("Sampler does not belong to this device.");
            sampler.ThrowIfDisposed();
        }

        private void ValidateQuery(AngleQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (!ReferenceEquals(query.Device, _device))
                throw new InvalidOperationException("Query does not belong to this device.");
            query.ThrowIfDisposed();
        }

        private void ValidateSync(AngleSync sync)
        {
            if (sync == null) throw new ArgumentNullException(nameof(sync));
            if (!ReferenceEquals(sync.Device, _device))
                throw new InvalidOperationException("Sync does not belong to this device.");
            sync.ThrowIfDisposed();
        }

        private void ValidateTransformFeedback(AngleTransformFeedback tf)
        {
            if (tf == null) throw new ArgumentNullException(nameof(tf));
            if (!ReferenceEquals(tf.Device, _device))
                throw new InvalidOperationException("Transform feedback does not belong to this device.");
            tf.ThrowIfDisposed();
        }
    }
}
