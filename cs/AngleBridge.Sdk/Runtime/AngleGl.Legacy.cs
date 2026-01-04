using AngleBridge.Managed.GLES;
using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    partial class AngleGl
    {
        [Obsolete("Use gl.State.Viewport instead.")]
        public void Viewport(int x, int y, int width, int height) => State.Viewport(x, y, width, height);

        [Obsolete("Use gl.State.ClearColor instead.")]
        public void ClearColor(float r, float g, float b, float a) => State.ClearColor(r, g, b, a);

        [Obsolete("Use gl.State.Clear instead.")]
        public void Clear(ClearBufferMask mask) => State.Clear(mask);

        [Obsolete("Use gl.State.ClearStencil instead.")]
        public void ClearStencil(int s) => State.ClearStencil(s);

        [Obsolete("Use gl.State.ClearDepth instead.")]
        public void ClearDepth(float depth) => State.ClearDepth(depth);

        [Obsolete("Use gl.State.Enable instead.")]
        public void Enable(EnableCap cap) => State.Enable(cap);

        [Obsolete("Use gl.State.Disable instead.")]
        public void Disable(EnableCap cap) => State.Disable(cap);

        [Obsolete("Use gl.State.BlendFunc instead.")]
        public void BlendFunc(BlendFactor sfactor, BlendFactor dfactor) => State.BlendFunc(sfactor, dfactor);

        [Obsolete("Use gl.State.StencilFunc instead.")]
        public void StencilFunc(StencilFunction func, int reference, int mask) => State.StencilFunc(func, reference, mask);

        [Obsolete("Use gl.State.StencilOp instead.")]
        public void StencilOp(StencilOp sfail, StencilOp dpfail, StencilOp dppass) => State.StencilOp(sfail, dpfail, dppass);

        [Obsolete("Use gl.State.StencilMask instead.")]
        public void StencilMask(int mask) => State.StencilMask(mask);

        [Obsolete("Use gl.Shaders.CreateShader instead.")]
        public GlShaderHandle CreateShader(ShaderType type) => Shaders.CreateShader(type);

        [Obsolete("Use gl.Shaders.CreateShaderObject instead.")]
        public AngleShader CreateShaderObject(ShaderType type) => Shaders.CreateShaderObject(type);

        [Obsolete("Use gl.Shaders.ShaderSource instead.")]
        public void ShaderSource(GlShaderHandle shader, string source) => Shaders.ShaderSource(shader, source);

        [Obsolete("Use gl.Shaders.ShaderSource instead.")]
        public void ShaderSource(AngleShader shader, string source) => Shaders.ShaderSource(shader, source);

        [Obsolete("Use gl.Shaders.CompileShader instead.")]
        public void CompileShader(GlShaderHandle shader) => Shaders.CompileShader(shader);

        [Obsolete("Use gl.Shaders.CompileShader instead.")]
        public void CompileShader(AngleShader shader) => Shaders.CompileShader(shader);

        [Obsolete("Use gl.Shaders.GetShaderInt instead.")]
        public int GetShaderInt(GlShaderHandle shader, ShaderParameter pname) => Shaders.GetShaderInt(shader, pname);

        [Obsolete("Use gl.Shaders.GetShaderInt instead.")]
        public int GetShaderInt(AngleShader shader, ShaderParameter pname) => Shaders.GetShaderInt(shader, pname);

        [Obsolete("Use gl.Shaders.GetShaderInfoLog instead.")]
        public string GetShaderInfoLog(GlShaderHandle shader) => Shaders.GetShaderInfoLog(shader);

        [Obsolete("Use gl.Shaders.GetShaderInfoLog instead.")]
        public string GetShaderInfoLog(AngleShader shader) => Shaders.GetShaderInfoLog(shader);

        [Obsolete("Use gl.Shaders.DeleteShader instead.")]
        public void DeleteShader(GlShaderHandle shader) => Shaders.DeleteShader(shader);

        [Obsolete("Use gl.Programs.CreateProgram instead.")]
        public GlProgramHandle CreateProgram() => Programs.CreateProgram();

        [Obsolete("Use gl.Programs.CreateProgramObject instead.")]
        public AngleProgram CreateProgramObject() => Programs.CreateProgramObject();

        [Obsolete("Use gl.Programs.AttachShader instead.")]
        public void AttachShader(GlProgramHandle program, GlShaderHandle shader) => Programs.AttachShader(program, shader);

        [Obsolete("Use gl.Programs.AttachShader instead.")]
        public void AttachShader(AngleProgram program, AngleShader shader) => Programs.AttachShader(program, shader);

        [Obsolete("Use gl.Programs.LinkProgram instead.")]
        public void LinkProgram(GlProgramHandle program) => Programs.LinkProgram(program);

        [Obsolete("Use gl.Programs.LinkProgram instead.")]
        public void LinkProgram(AngleProgram program) => Programs.LinkProgram(program);

        [Obsolete("Use gl.Programs.GetProgramInt instead.")]
        public int GetProgramInt(GlProgramHandle program, ProgramParameter pname) => Programs.GetProgramInt(program, pname);

        [Obsolete("Use gl.Programs.GetProgramInt instead.")]
        public int GetProgramInt(AngleProgram program, ProgramParameter pname) => Programs.GetProgramInt(program, pname);

        [Obsolete("Use gl.Programs.GetProgramInfoLog instead.")]
        public string GetProgramInfoLog(GlProgramHandle program) => Programs.GetProgramInfoLog(program);

        [Obsolete("Use gl.Programs.GetProgramInfoLog instead.")]
        public string GetProgramInfoLog(AngleProgram program) => Programs.GetProgramInfoLog(program);

        [Obsolete("Use gl.Programs.UseProgram instead.")]
        public void UseProgram(GlProgramHandle program) => Programs.UseProgram(program);

        [Obsolete("Use gl.Programs.UseProgram instead.")]
        public void UseProgram(AngleProgram program) => Programs.UseProgram(program);

        [Obsolete("Use gl.Programs.DeleteProgram instead.")]
        public void DeleteProgram(GlProgramHandle program) => Programs.DeleteProgram(program);

        [Obsolete("Use gl.Programs.GetAttribLocation instead.")]
        public int GetAttribLocation(GlProgramHandle program, string name) => Programs.GetAttribLocation(program, name);

        [Obsolete("Use gl.Programs.GetAttribLocation instead.")]
        public int GetAttribLocation(AngleProgram program, string name) => Programs.GetAttribLocation(program, name);

        [Obsolete("Use gl.Programs.GetUniformLocation instead.")]
        public int GetUniformLocation(GlProgramHandle program, string name) => Programs.GetUniformLocation(program, name);

        [Obsolete("Use gl.Programs.GetUniformLocation instead.")]
        public int GetUniformLocation(AngleProgram program, string name) => Programs.GetUniformLocation(program, name);

        [Obsolete("Use gl.Programs.Uniform1i instead.")]
        public void Uniform1i(int location, int v0) => Programs.Uniform1i(location, v0);

        [Obsolete("Use gl.Programs.Uniform1f instead.")]
        public void Uniform1f(int location, float v0) => Programs.Uniform1f(location, v0);

        [Obsolete("Use gl.Programs.Uniform3f instead.")]
        public void Uniform3f(int location, float v0, float v1, float v2) => Programs.Uniform3f(location, v0, v1, v2);

        [Obsolete("Use gl.Programs.Uniform4f instead.")]
        public void Uniform4f(int location, float v0, float v1, float v2, float v3) => Programs.Uniform4f(location, v0, v1, v2, v3);

        [Obsolete("Use gl.Programs.UniformMatrix4fv instead.")]
        public void UniformMatrix4fv(int location, bool transpose, float[] value) => Programs.UniformMatrix4fv(location, transpose, value);

        [Obsolete("Use gl.Buffers.GenBuffer instead.")]
        public GlBufferHandle GenBuffer() => Buffers.GenBuffer();

        [Obsolete("Use gl.Buffers.CreateBufferObject instead.")]
        public AngleBuffer CreateBufferObject() => Buffers.CreateBufferObject();

        [Obsolete("Use gl.Buffers.DeleteBuffer instead.")]
        public void DeleteBuffer(GlBufferHandle buffer) => Buffers.DeleteBuffer(buffer);

        [Obsolete("Use gl.Buffers.BindBuffer instead.")]
        public void BindBuffer(BufferTarget target, GlBufferHandle buffer) => Buffers.BindBuffer(target, buffer);

        [Obsolete("Use gl.Buffers.BindBuffer instead.")]
        public void BindBuffer(BufferTarget target, AngleBuffer buffer) => Buffers.BindBuffer(target, buffer);

        [Obsolete("Use gl.Buffers.BufferData instead.")]
        public void BufferData(BufferTarget target, float[] data, BufferUsage usage) => Buffers.BufferData(target, data, usage);

        [Obsolete("Use gl.Buffers.BufferData instead.")]
        [CLSCompliant(false)]
        public void BufferData(BufferTarget target, ushort[] data, BufferUsage usage) => Buffers.BufferData(target, data, usage);

        [Obsolete("Use gl.VertexArrays.EnableVertexAttribArray instead.")]
        public void EnableVertexAttribArray(int index) => VertexArrays.EnableVertexAttribArray(index);

        [Obsolete("Use gl.VertexArrays.VertexAttribPointer instead.")]
        public void VertexAttribPointer(int index, int size, VertexAttribType type, bool normalized, int stride, int offsetBytes) => VertexArrays.VertexAttribPointer(index, size, type, normalized, stride, offsetBytes);

        [Obsolete("Use gl.Draw.DrawArrays instead.")]
        public void DrawArrays(PrimitiveType mode, int first, int count) => Draw.DrawArrays(mode, first, count);

        [Obsolete("Use gl.Draw.DrawElements instead.")]
        public void DrawElements(PrimitiveType mode, int count, IndexElementType type, int offsetBytes) => Draw.DrawElements(mode, count, type, offsetBytes);

        [Obsolete("Use gl.Textures.GenTexture instead.")]
        public GlTextureHandle GenTexture() => Textures.GenTexture();

        [Obsolete("Use gl.Textures.CreateTextureObject instead.")]
        public AngleTexture CreateTextureObject() => Textures.CreateTextureObject();

        [Obsolete("Use gl.Textures.DeleteTexture instead.")]
        public void DeleteTexture(GlTextureHandle texture) => Textures.DeleteTexture(texture);

        [Obsolete("Use gl.Textures.BindTexture instead.")]
        public void BindTexture(TextureTarget target, GlTextureHandle texture) => Textures.BindTexture(target, texture);

        [Obsolete("Use gl.Textures.BindTexture instead.")]
        public void BindTexture(TextureTarget target, AngleTexture texture) => Textures.BindTexture(target, texture);

        [Obsolete("Use gl.Textures.ActiveTexture instead.")]
        public void ActiveTexture(TextureUnit texture) => Textures.ActiveTexture(texture);

        [Obsolete("Use gl.Textures.TexParameteri instead.")]
        public void TexParameteri(TextureTarget target, TextureParameterName pname, int param) => Textures.TexParameteri(target, pname, param);

        [Obsolete("Use gl.Textures.TexParameterf instead.")]
        public void TexParameterf(TextureTarget target, TextureParameterName pname, float param) => Textures.TexParameterf(target, pname, param);

        [Obsolete("Use gl.Textures.PixelStore instead.")]
        public void PixelStore(PixelStoreParameter pname, int param) => Textures.PixelStore(pname, param);

        [Obsolete("Use gl.Textures.TexImage2D instead.")]
        public void TexImage2D(TextureTarget target, int level, TextureInternalFormat internalFormat, int width, int height, int border, PixelFormat format, PixelType type, byte[]? pixels) => Textures.TexImage2D(target, level, internalFormat, width, height, border, format, type, pixels);

        [Obsolete("Use gl.State.CullFace instead.")]
        public void CullFace(CullFaceMode mode) => State.CullFace(mode);

        [Obsolete("Use gl.Textures.GenerateMipmap instead.")]
        public void GenerateMipmap(TextureTarget target) => Textures.GenerateMipmap(target);

        [Obsolete("Use gl.Framebuffers.GenFramebuffer instead.")]
        public GlFramebufferHandle GenFramebuffer() => Framebuffers.GenFramebuffer();

        [Obsolete("Use gl.Framebuffers.CreateFramebufferObject instead.")]
        public AngleFramebuffer CreateFramebufferObject() => Framebuffers.CreateFramebufferObject();

        [Obsolete("Use gl.Framebuffers.DeleteFramebuffer instead.")]
        public void DeleteFramebuffer(GlFramebufferHandle framebuffer) => Framebuffers.DeleteFramebuffer(framebuffer);

        [Obsolete("Use gl.Framebuffers.DeleteFramebuffer instead.")]
        public void DeleteFramebuffer(AngleFramebuffer framebuffer) => Framebuffers.DeleteFramebuffer(framebuffer);

        [Obsolete("Use gl.Framebuffers.BindFramebuffer instead.")]
        public void BindFramebuffer(FramebufferTarget target, GlFramebufferHandle framebuffer) => Framebuffers.BindFramebuffer(target, framebuffer);

        [Obsolete("Use gl.Framebuffers.BindFramebuffer instead.")]
        public void BindFramebuffer(FramebufferTarget target, AngleFramebuffer framebuffer) => Framebuffers.BindFramebuffer(target, framebuffer);

        [Obsolete("Use gl.Framebuffers.FramebufferTexture2D instead.")]
        public void FramebufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, GlTextureHandle texture, int level) => Framebuffers.FramebufferTexture2D(target, attachment, textarget, texture, level);

        [Obsolete("Use gl.Framebuffers.FramebufferTexture2D instead.")]
        public void FramebufferTexture2D(FramebufferTarget target, FramebufferAttachment attachment, TextureTarget textarget, AngleTexture texture, int level) => Framebuffers.FramebufferTexture2D(target, attachment, textarget, texture, level);

        [Obsolete("Use gl.Framebuffers.FramebufferRenderbuffer instead.")]
        public void FramebufferRenderbuffer(FramebufferTarget target, FramebufferAttachment attachment, RenderbufferTarget renderbufferTarget, GlRenderbufferHandle renderbuffer) => Framebuffers.FramebufferRenderbuffer(target, attachment, renderbufferTarget, renderbuffer);

        [Obsolete("Use gl.Framebuffers.FramebufferRenderbuffer instead.")]
        public void FramebufferRenderbuffer(FramebufferTarget target, FramebufferAttachment attachment, RenderbufferTarget renderbufferTarget, AngleRenderbuffer renderbuffer) => Framebuffers.FramebufferRenderbuffer(target, attachment, renderbufferTarget, renderbuffer);

        [Obsolete("Use gl.Framebuffers.CheckFramebufferStatus instead.")]
        public FramebufferStatus CheckFramebufferStatus(FramebufferTarget target) => Framebuffers.CheckFramebufferStatus(target);

        [Obsolete("Use gl.Renderbuffers.GenRenderbuffer instead.")]
        public GlRenderbufferHandle GenRenderbuffer() => Renderbuffers.GenRenderbuffer();

        [Obsolete("Use gl.Renderbuffers.CreateRenderbufferObject instead.")]
        public AngleRenderbuffer CreateRenderbufferObject() => Renderbuffers.CreateRenderbufferObject();

        [Obsolete("Use gl.Renderbuffers.DeleteRenderbuffer instead.")]
        public void DeleteRenderbuffer(GlRenderbufferHandle renderbuffer) => Renderbuffers.DeleteRenderbuffer(renderbuffer);

        [Obsolete("Use gl.Renderbuffers.DeleteRenderbuffer instead.")]
        public void DeleteRenderbuffer(AngleRenderbuffer renderbuffer) => Renderbuffers.DeleteRenderbuffer(renderbuffer);

        [Obsolete("Use gl.Renderbuffers.BindRenderbuffer instead.")]
        public void BindRenderbuffer(RenderbufferTarget target, GlRenderbufferHandle renderbuffer) => Renderbuffers.BindRenderbuffer(target, renderbuffer);

        [Obsolete("Use gl.Renderbuffers.BindRenderbuffer instead.")]
        public void BindRenderbuffer(RenderbufferTarget target, AngleRenderbuffer renderbuffer) => Renderbuffers.BindRenderbuffer(target, renderbuffer);

        [Obsolete("Use gl.Renderbuffers.RenderbufferStorage instead.")]
        public void RenderbufferStorage(RenderbufferTarget target, RenderbufferInternalFormat internalFormat, int width, int height) => Renderbuffers.RenderbufferStorage(target, internalFormat, width, height);

        [Obsolete("Use gl.Framebuffers.BlitFramebuffer instead.")]
        public void BlitFramebuffer(int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, ClearBufferMask mask, TextureMagFilter filter) => Framebuffers.BlitFramebuffer(srcX0, srcY0, srcX1, srcY1, dstX0, dstY0, dstX1, dstY1, mask, filter);

        [Obsolete("Use gl.Samplers.GenSampler instead.")]
        public GlSamplerHandle GenSampler() => Samplers.GenSampler();

        [Obsolete("Use gl.Samplers.CreateSamplerObject instead.")]
        public AngleSampler CreateSamplerObject() => Samplers.CreateSamplerObject();

        [Obsolete("Use gl.Samplers.DeleteSampler instead.")]
        public void DeleteSampler(GlSamplerHandle sampler) => Samplers.DeleteSampler(sampler);

        [Obsolete("Use gl.Samplers.DeleteSampler instead.")]
        public void DeleteSampler(AngleSampler sampler) => Samplers.DeleteSampler(sampler);

        [Obsolete("Use gl.Samplers.BindSampler instead.")]
        public void BindSampler(int unit, GlSamplerHandle sampler) => Samplers.BindSampler(unit, sampler);

        [Obsolete("Use gl.Samplers.BindSampler instead.")]
        public void BindSampler(int unit, AngleSampler sampler) => Samplers.BindSampler(unit, sampler);

        [Obsolete("Use gl.Samplers.SamplerParameteri instead.")]
        public void SamplerParameteri(GlSamplerHandle sampler, TextureParameterName pname, int param) => Samplers.SamplerParameteri(sampler, pname, param);

        [Obsolete("Use gl.Samplers.SamplerParameteri instead.")]
        public void SamplerParameteri(AngleSampler sampler, TextureParameterName pname, int param) => Samplers.SamplerParameteri(sampler, pname, param);

        [Obsolete("Use gl.Samplers.SamplerParameterf instead.")]
        public void SamplerParameterf(GlSamplerHandle sampler, TextureParameterName pname, float param) => Samplers.SamplerParameterf(sampler, pname, param);

        [Obsolete("Use gl.Samplers.SamplerParameterf instead.")]
        public void SamplerParameterf(AngleSampler sampler, TextureParameterName pname, float param) => Samplers.SamplerParameterf(sampler, pname, param);

        [Obsolete("Use gl.VertexArrays.GenVertexArray instead.")]
        public GlVertexArrayHandle GenVertexArray() => VertexArrays.GenVertexArray();

        [Obsolete("Use gl.VertexArrays.CreateVertexArrayObject instead.")]
        public AngleVertexArray CreateVertexArrayObject() => VertexArrays.CreateVertexArrayObject();

        [Obsolete("Use gl.VertexArrays.BindVertexArray instead.")]
        public void BindVertexArray(GlVertexArrayHandle vao) => VertexArrays.BindVertexArray(vao);

        [Obsolete("Use gl.VertexArrays.BindVertexArray instead.")]
        public void BindVertexArray(AngleVertexArray vao) => VertexArrays.BindVertexArray(vao);

        [Obsolete("Use gl.VertexArrays.DeleteVertexArray instead.")]
        public void DeleteVertexArray(GlVertexArrayHandle vao) => VertexArrays.DeleteVertexArray(vao);

        [Obsolete("Use gl.Draw.Finish instead.")]
        public void Finish() => Draw.Finish();

        [Obsolete("Use gl.Queries.IsDisjointTimerQueryAvailable instead.")]
        public bool IsDisjointTimerQueryAvailable => Queries.IsDisjointTimerQueryAvailable;

        [Obsolete("Use gl.Queries.CreateQueryObject instead.")]
        public AngleQuery CreateQueryObject() => Queries.CreateQueryObject();

        [Obsolete("Use gl.Queries.DeleteQuery instead.")]
        public void DeleteQuery(GlQueryHandle query) => Queries.DeleteQuery(query);

        [Obsolete("Use gl.Queries.DeleteQuery instead.")]
        public void DeleteQuery(AngleQuery query) => Queries.DeleteQuery(query);

        [Obsolete("Use gl.Queries.BeginQuery instead.")]
        public void BeginQuery(QueryTarget target, AngleQuery query) => Queries.BeginQuery(target, query);

        [Obsolete("Use gl.Queries.EndQuery instead.")]
        public void EndQuery(QueryTarget target) => Queries.EndQuery(target);

        [Obsolete("Use gl.Queries.TryGetQueryResultAvailable instead.")]
        public bool TryGetQueryResultAvailable(AngleQuery query, out bool available) => Queries.TryGetQueryResultAvailable(query, out available);

        [Obsolete("Use gl.Queries.TryGetQueryResult instead.")]
        [CLSCompliant(false)]
        public bool TryGetQueryResult(AngleQuery query, out ulong result) => Queries.TryGetQueryResult(query, out result);

        [Obsolete("Use gl.Caps.TryGetGpuDisjoint instead.")]
        public bool TryGetGpuDisjoint(out bool disjoint) => Caps.TryGetGpuDisjoint(out disjoint);

        [Obsolete("Use gl.Sync.FenceSync instead.")]
        public AngleSync FenceSync(SyncCondition condition = SyncCondition.SyncGpuCommandsComplete, SyncFlags flags = SyncFlags.None) => Sync.FenceSync(condition, flags);

        [Obsolete("Use gl.Sync.DeleteSync instead.")]
        public void DeleteSync(AngleSync sync) => Sync.DeleteSync(sync);

        [Obsolete("Use gl.Sync.ClientWaitSync instead.")]
        [CLSCompliant(false)]
        public ClientWaitSyncResult ClientWaitSync(AngleSync sync, SyncFlags flags, ulong timeoutNanoseconds) => Sync.ClientWaitSync(sync, flags, timeoutNanoseconds);

        [Obsolete("Use gl.Sync.WaitSync instead.")]
        [CLSCompliant(false)]
        public void WaitSync(AngleSync sync, SyncFlags flags, ulong timeoutNanoseconds) => Sync.WaitSync(sync, flags, timeoutNanoseconds);

        [Obsolete("Use gl.Sync.DeleteSync instead.")]
        [CLSCompliant(false)]
        public void DeleteSync(GlSyncHandle sync) => Sync.DeleteSync(sync);

        [Obsolete("Use gl.Queries.CreateTimerQuery instead.")]
        public int CreateTimerQuery() => Queries.CreateTimerQuery();

        [Obsolete("Use gl.Queries.DeleteTimerQuery instead.")]
        public void DeleteTimerQuery(int query) => Queries.DeleteTimerQuery(query);

        [Obsolete("Use gl.Queries.BeginTimerQuery instead.")]
        public void BeginTimerQuery(int query) => Queries.BeginTimerQuery(query);

        [Obsolete("Use gl.Queries.EndTimerQuery instead.")]
        public void EndTimerQuery() => Queries.EndTimerQuery();

        [Obsolete("Use gl.Queries.TryGetTimerQueryResultAvailable instead.")]
        public bool TryGetTimerQueryResultAvailable(int query, out bool available) => Queries.TryGetTimerQueryResultAvailable(query, out available);

        [Obsolete("Use gl.Queries.TryGetTimerQueryResult instead.")]
        public bool TryGetTimerQueryResult(int query, out long nanoseconds) => Queries.TryGetTimerQueryResult(query, out nanoseconds);

        [Obsolete("Use gl.Caps.GetInteger instead.")]
        public int GetInteger(GetIntegerParameter pname) => Caps.GetInteger(pname);

        [Obsolete("Use gl.Caps.TryGetInteger instead.")]
        public bool TryGetInteger(GetIntegerParameter pname, out int value) => Caps.TryGetInteger(pname, out value);

        [Obsolete("Use gl.Caps.GetFloat instead.")]
        public float GetFloat(GetFloatParameter pname) => Caps.GetFloat(pname);

        [Obsolete("Use gl.Caps.TryGetFloat instead.")]
        public bool TryGetFloat(GetFloatParameter pname, out float value) => Caps.TryGetFloat(pname, out value);

        [Obsolete("Use gl.TransformFeedback.CreateTransformFeedbackObject instead.")]
        public AngleTransformFeedback CreateTransformFeedbackObject() => TransformFeedback.CreateTransformFeedbackObject();

        [Obsolete("Use gl.TransformFeedback.GenTransformFeedback instead.")]
        public GlTransformFeedbackHandle GenTransformFeedback() => TransformFeedback.GenTransformFeedback();

        [Obsolete("Use gl.TransformFeedback.DeleteTransformFeedback instead.")]
        public void DeleteTransformFeedback(GlTransformFeedbackHandle tf) => TransformFeedback.DeleteTransformFeedback(tf);

        [Obsolete("Use gl.TransformFeedback.DeleteTransformFeedback instead.")]
        public void DeleteTransformFeedback(AngleTransformFeedback tf) => TransformFeedback.DeleteTransformFeedback(tf);

        [Obsolete("Use gl.TransformFeedback.BindTransformFeedback instead.")]
        public void BindTransformFeedback(TransformFeedbackTarget target, GlTransformFeedbackHandle tf) => TransformFeedback.BindTransformFeedback(target, tf);

        [Obsolete("Use gl.TransformFeedback.BindTransformFeedback instead.")]
        public void BindTransformFeedback(TransformFeedbackTarget target, AngleTransformFeedback tf) => TransformFeedback.BindTransformFeedback(target, tf);

        [Obsolete("Use gl.TransformFeedback.BeginTransformFeedback instead.")]
        public void BeginTransformFeedback(PrimitiveType primitiveMode) => TransformFeedback.BeginTransformFeedback(primitiveMode);

        [Obsolete("Use gl.TransformFeedback.EndTransformFeedback instead.")]
        public void EndTransformFeedback() => TransformFeedback.EndTransformFeedback();

        [Obsolete("Use gl.TransformFeedback.PauseTransformFeedback instead.")]
        public void PauseTransformFeedback() => TransformFeedback.PauseTransformFeedback();

        [Obsolete("Use gl.TransformFeedback.ResumeTransformFeedback instead.")]
        public void ResumeTransformFeedback() => TransformFeedback.ResumeTransformFeedback();

        [Obsolete("Use gl.TransformFeedback.TransformFeedbackVaryings instead.")]
        public void TransformFeedbackVaryings(AngleProgram program, string[] varyings, TransformFeedbackBufferMode bufferMode) => TransformFeedback.TransformFeedbackVaryings(program, varyings, bufferMode);

        [Obsolete("Use gl.TransformFeedback.TryGetTransformFeedbackVarying instead.")]
        [CLSCompliant(false)]
        public bool TryGetTransformFeedbackVarying(AngleProgram program, int index, byte[]? nameBufferUtf8, out int length, out int size, out uint type) => TransformFeedback.TryGetTransformFeedbackVarying(program, index, nameBufferUtf8, out length, out size, out type);

        [Obsolete("Use gl.Buffers.BindBufferBase instead.")]
        [CLSCompliant(false)]
        public void BindBufferBase(BufferTarget target, uint index, GlBufferHandle buffer) => Buffers.BindBufferBase(target, index, buffer);

        [Obsolete("Use gl.Buffers.BindBufferBase instead.")]
        [CLSCompliant(false)]
        public void BindBufferBase(BufferTarget target, uint index, AngleBuffer buffer) => Buffers.BindBufferBase(target, index, buffer);

        [Obsolete("Use gl.Buffers.BindBufferRange instead.")]
        [CLSCompliant(false)]
        public void BindBufferRange(BufferTarget target, uint index, GlBufferHandle buffer, nint offset, nint size) => Buffers.BindBufferRange(target, index, buffer, offset, size);

        [Obsolete("Use gl.Buffers.BindBufferRange instead.")]
        [CLSCompliant(false)]
        public void BindBufferRange(BufferTarget target, uint index, AngleBuffer buffer, nint offset, nint size) => Buffers.BindBufferRange(target, index, buffer, offset, size);
    }
}
