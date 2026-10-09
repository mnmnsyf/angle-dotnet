using System;
using System.Runtime.InteropServices;

namespace AngleBridge.Managed.GLES
{
    internal static partial class Gles
    {
        internal static glBindAttribLocation_t? _glBindAttribLocation;
        internal static glBlendColor_t? _glBlendColor;
        internal static glBlendEquation_t? _glBlendEquation;
        internal static glBlendEquationSeparate_t? _glBlendEquationSeparate;
        internal static glBlendFuncSeparate_t? _glBlendFuncSeparate;
        internal static glBufferSubData_t? _glBufferSubData;
        internal static glClearBufferfi_t? _glClearBufferfi;
        internal static glClearBufferfv_t? _glClearBufferfv;
        internal static glClearBufferiv_t? _glClearBufferiv;
        internal static glClearBufferuiv_t? _glClearBufferuiv;
        internal static glColorMask_t? _glColorMask;
        internal static glCompressedTexImage2D_t? _glCompressedTexImage2D;
        internal static glCompressedTexImage3D_t? _glCompressedTexImage3D;
        internal static glCompressedTexSubImage2D_t? _glCompressedTexSubImage2D;
        internal static glCompressedTexSubImage3D_t? _glCompressedTexSubImage3D;
        internal static glCopyBufferSubData_t? _glCopyBufferSubData;
        internal static glCopyTexImage2D_t? _glCopyTexImage2D;
        internal static glCopyTexSubImage2D_t? _glCopyTexSubImage2D;
        internal static glCopyTexSubImage3D_t? _glCopyTexSubImage3D;
        internal static glDepthFunc_t? _glDepthFunc;
        internal static glDepthMask_t? _glDepthMask;
        internal static glDepthRangef_t? _glDepthRangef;
        internal static glDetachShader_t? _glDetachShader;
        internal static glDisableVertexAttribArray_t? _glDisableVertexAttribArray;
        internal static glDrawArraysInstanced_t? _glDrawArraysInstanced;
        internal static glDrawElementsInstanced_t? _glDrawElementsInstanced;
        internal static glDrawRangeElements_t? _glDrawRangeElements;
        internal static glFlush_t? _glFlush;
        internal static glFlushMappedBufferRange_t? _glFlushMappedBufferRange;
        internal static glFramebufferTextureLayer_t? _glFramebufferTextureLayer;
        internal static glFrontFace_t? _glFrontFace;
        internal static glGetActiveAttrib_t? _glGetActiveAttrib;
        internal static glGetActiveUniform_t? _glGetActiveUniform;
        internal static glGetActiveUniformBlockName_t? _glGetActiveUniformBlockName;
        internal static glGetActiveUniformBlockiv_t? _glGetActiveUniformBlockiv;
        internal static glGetActiveUniformsiv_t? _glGetActiveUniformsiv;
        internal static glGetAttachedShaders_t? _glGetAttachedShaders;
        internal static glGetBufferParameteri64v_t? _glGetBufferParameteri64v;
        internal static glGetBufferParameteriv_t? _glGetBufferParameteriv;
        internal static glGetBufferPointerv_t? _glGetBufferPointerv;
        internal static glGetFragDataLocation_t? _glGetFragDataLocation;
        internal static glGetFramebufferAttachmentParameteriv_t? _glGetFramebufferAttachmentParameteriv;
        internal static glGetInteger64i_v_t? _glGetInteger64i_v;
        internal static glGetInteger64v_t? _glGetInteger64v;
        internal static glGetIntegeri_v_t? _glGetIntegeri_v;
        internal static glGetInternalformativ_t? _glGetInternalformativ;
        internal static glGetProgramBinary_t? _glGetProgramBinary;
        internal static glGetQueryiv_t? _glGetQueryiv;
        internal static glGetRenderbufferParameteriv_t? _glGetRenderbufferParameteriv;
        internal static glGetSamplerParameterfv_t? _glGetSamplerParameterfv;
        internal static glGetSamplerParameteriv_t? _glGetSamplerParameteriv;
        internal static glGetShaderPrecisionFormat_t? _glGetShaderPrecisionFormat;
        internal static glGetShaderSource_t? _glGetShaderSource;
        internal static glGetString_t? _glGetString;
        internal static glGetStringi_t? _glGetStringi;
        internal static glGetTexParameterfv_t? _glGetTexParameterfv;
        internal static glGetTexParameteriv_t? _glGetTexParameteriv;
        internal static glGetUniformBlockIndex_t? _glGetUniformBlockIndex;
        internal static glGetUniformIndices_t? _glGetUniformIndices;
        internal static glGetUniformfv_t? _glGetUniformfv;
        internal static glGetUniformiv_t? _glGetUniformiv;
        internal static glGetUniformuiv_t? _glGetUniformuiv;
        internal static glGetVertexAttribIiv_t? _glGetVertexAttribIiv;
        internal static glGetVertexAttribIuiv_t? _glGetVertexAttribIuiv;
        internal static glGetVertexAttribPointerv_t? _glGetVertexAttribPointerv;
        internal static glGetVertexAttribfv_t? _glGetVertexAttribfv;
        internal static glGetVertexAttribiv_t? _glGetVertexAttribiv;
        internal static glHint_t? _glHint;
        internal static glInvalidateFramebuffer_t? _glInvalidateFramebuffer;
        internal static glInvalidateSubFramebuffer_t? _glInvalidateSubFramebuffer;
        internal static glIsBuffer_t? _glIsBuffer;
        internal static glIsEnabled_t? _glIsEnabled;
        internal static glIsFramebuffer_t? _glIsFramebuffer;
        internal static glIsProgram_t? _glIsProgram;
        internal static glIsQuery_t? _glIsQuery;
        internal static glIsRenderbuffer_t? _glIsRenderbuffer;
        internal static glIsSampler_t? _glIsSampler;
        internal static glIsShader_t? _glIsShader;
        internal static glIsTexture_t? _glIsTexture;
        internal static glIsTransformFeedback_t? _glIsTransformFeedback;
        internal static glIsVertexArray_t? _glIsVertexArray;
        internal static glLineWidth_t? _glLineWidth;
        internal static glMapBufferRange_t? _glMapBufferRange;
        internal static glPolygonOffset_t? _glPolygonOffset;
        internal static glProgramBinary_t? _glProgramBinary;
        internal static glProgramParameteri_t? _glProgramParameteri;
        internal static glReadBuffer_t? _glReadBuffer;
        internal static glReleaseShaderCompiler_t? _glReleaseShaderCompiler;
        internal static glRenderbufferStorageMultisample_t? _glRenderbufferStorageMultisample;
        internal static glSampleCoverage_t? _glSampleCoverage;
        internal static glSamplerParameterfv_t? _glSamplerParameterfv;
        internal static glSamplerParameteriv_t? _glSamplerParameteriv;
        internal static glScissor_t? _glScissor;
        internal static glShaderBinary_t? _glShaderBinary;
        internal static glStencilFuncSeparate_t? _glStencilFuncSeparate;
        internal static glStencilMaskSeparate_t? _glStencilMaskSeparate;
        internal static glStencilOpSeparate_t? _glStencilOpSeparate;
        internal static glTexParameterfv_t? _glTexParameterfv;
        internal static glTexParameteriv_t? _glTexParameteriv;
        internal static glTexStorage2D_t? _glTexStorage2D;
        internal static glTexStorage3D_t? _glTexStorage3D;
        internal static glTexSubImage2D_t? _glTexSubImage2D;
        internal static glTexSubImage3D_t? _glTexSubImage3D;
        internal static glUniform1fv_t? _glUniform1fv;
        internal static glUniform1iv_t? _glUniform1iv;
        internal static glUniform1ui_t? _glUniform1ui;
        internal static glUniform1uiv_t? _glUniform1uiv;
        internal static glUniform2f_t? _glUniform2f;
        internal static glUniform2fv_t? _glUniform2fv;
        internal static glUniform2i_t? _glUniform2i;
        internal static glUniform2iv_t? _glUniform2iv;
        internal static glUniform2ui_t? _glUniform2ui;
        internal static glUniform2uiv_t? _glUniform2uiv;
        internal static glUniform3fv_t? _glUniform3fv;
        internal static glUniform3i_t? _glUniform3i;
        internal static glUniform3iv_t? _glUniform3iv;
        internal static glUniform3ui_t? _glUniform3ui;
        internal static glUniform3uiv_t? _glUniform3uiv;
        internal static glUniform4fv_t? _glUniform4fv;
        internal static glUniform4i_t? _glUniform4i;
        internal static glUniform4iv_t? _glUniform4iv;
        internal static glUniform4ui_t? _glUniform4ui;
        internal static glUniform4uiv_t? _glUniform4uiv;
        internal static glUniformBlockBinding_t? _glUniformBlockBinding;
        internal static glUniformMatrix2fv_t? _glUniformMatrix2fv;
        internal static glUniformMatrix2x3fv_t? _glUniformMatrix2x3fv;
        internal static glUniformMatrix2x4fv_t? _glUniformMatrix2x4fv;
        internal static glUniformMatrix3fv_t? _glUniformMatrix3fv;
        internal static glUniformMatrix3x2fv_t? _glUniformMatrix3x2fv;
        internal static glUniformMatrix3x4fv_t? _glUniformMatrix3x4fv;
        internal static glUniformMatrix4x2fv_t? _glUniformMatrix4x2fv;
        internal static glUniformMatrix4x3fv_t? _glUniformMatrix4x3fv;
        internal static glUnmapBuffer_t? _glUnmapBuffer;
        internal static glValidateProgram_t? _glValidateProgram;
        internal static glVertexAttrib1f_t? _glVertexAttrib1f;
        internal static glVertexAttrib1fv_t? _glVertexAttrib1fv;
        internal static glVertexAttrib2f_t? _glVertexAttrib2f;
        internal static glVertexAttrib2fv_t? _glVertexAttrib2fv;
        internal static glVertexAttrib3f_t? _glVertexAttrib3f;
        internal static glVertexAttrib3fv_t? _glVertexAttrib3fv;
        internal static glVertexAttrib4f_t? _glVertexAttrib4f;
        internal static glVertexAttrib4fv_t? _glVertexAttrib4fv;
        internal static glVertexAttribDivisor_t? _glVertexAttribDivisor;
        internal static glVertexAttribI4i_t? _glVertexAttribI4i;
        internal static glVertexAttribI4iv_t? _glVertexAttribI4iv;
        internal static glVertexAttribI4ui_t? _glVertexAttribI4ui;
        internal static glVertexAttribI4uiv_t? _glVertexAttribI4uiv;
        internal static glVertexAttribIPointer_t? _glVertexAttribIPointer;

        public static void BindAttribLocation(uint program, uint index, string name) =>
            (_glBindAttribLocation ?? throw NotLoaded(nameof(_glBindAttribLocation)))(program, index, name);

        public static void BlendColor(float red, float green, float blue, float alpha) =>
            (_glBlendColor ?? throw NotLoaded(nameof(_glBlendColor)))(red, green, blue, alpha);

        public static void BlendEquation(uint mode) =>
            (_glBlendEquation ?? throw NotLoaded(nameof(_glBlendEquation)))(mode);

        public static void BlendEquationSeparate(uint modeRGB, uint modeAlpha) =>
            (_glBlendEquationSeparate ?? throw NotLoaded(nameof(_glBlendEquationSeparate)))(modeRGB, modeAlpha);

        public static void BlendFuncSeparate(uint sfactorRGB, uint dfactorRGB, uint sfactorAlpha, uint dfactorAlpha) =>
            (_glBlendFuncSeparate ?? throw NotLoaded(nameof(_glBlendFuncSeparate)))(sfactorRGB, dfactorRGB, sfactorAlpha, dfactorAlpha);

        public static void BufferSubData(uint target, nint offset, nint size, IntPtr data) =>
            (_glBufferSubData ?? throw NotLoaded(nameof(_glBufferSubData)))(target, offset, size, data);

        public static void ClearBufferfi(uint buffer, int drawbuffer, float depth, int stencil) =>
            (_glClearBufferfi ?? throw NotSupported(nameof(_glClearBufferfi), "Requires GLES3.0."))(buffer, drawbuffer, depth, stencil);

        public static void ClearBufferfv(uint buffer, int drawbuffer, IntPtr value) =>
            (_glClearBufferfv ?? throw NotSupported(nameof(_glClearBufferfv), "Requires GLES3.0."))(buffer, drawbuffer, value);

        public static void ClearBufferiv(uint buffer, int drawbuffer, IntPtr value) =>
            (_glClearBufferiv ?? throw NotSupported(nameof(_glClearBufferiv), "Requires GLES3.0."))(buffer, drawbuffer, value);

        public static void ClearBufferuiv(uint buffer, int drawbuffer, IntPtr value) =>
            (_glClearBufferuiv ?? throw NotSupported(nameof(_glClearBufferuiv), "Requires GLES3.0."))(buffer, drawbuffer, value);

        public static void ColorMask(byte red, byte green, byte blue, byte alpha) =>
            (_glColorMask ?? throw NotLoaded(nameof(_glColorMask)))(red, green, blue, alpha);

        public static void CompressedTexImage2D(uint target, int level, uint internalformat, int width, int height, int border, int imageSize, IntPtr data) =>
            (_glCompressedTexImage2D ?? throw NotLoaded(nameof(_glCompressedTexImage2D)))(target, level, internalformat, width, height, border, imageSize, data);

        public static void CompressedTexImage3D(uint target, int level, uint internalformat, int width, int height, int depth, int border, int imageSize, IntPtr data) =>
            (_glCompressedTexImage3D ?? throw NotSupported(nameof(_glCompressedTexImage3D), "Requires GLES3.0."))(target, level, internalformat, width, height, depth, border, imageSize, data);

        public static void CompressedTexSubImage2D(uint target, int level, int xoffset, int yoffset, int width, int height, uint format, int imageSize, IntPtr data) =>
            (_glCompressedTexSubImage2D ?? throw NotLoaded(nameof(_glCompressedTexSubImage2D)))(target, level, xoffset, yoffset, width, height, format, imageSize, data);

        public static void CompressedTexSubImage3D(uint target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, uint format, int imageSize, IntPtr data) =>
            (_glCompressedTexSubImage3D ?? throw NotSupported(nameof(_glCompressedTexSubImage3D), "Requires GLES3.0."))(target, level, xoffset, yoffset, zoffset, width, height, depth, format, imageSize, data);

        public static void CopyBufferSubData(uint readTarget, uint writeTarget, nint readOffset, nint writeOffset, nint size) =>
            (_glCopyBufferSubData ?? throw NotSupported(nameof(_glCopyBufferSubData), "Requires GLES3.0."))(readTarget, writeTarget, readOffset, writeOffset, size);

        public static void CopyTexImage2D(uint target, int level, uint internalformat, int x, int y, int width, int height, int border) =>
            (_glCopyTexImage2D ?? throw NotLoaded(nameof(_glCopyTexImage2D)))(target, level, internalformat, x, y, width, height, border);

        public static void CopyTexSubImage2D(uint target, int level, int xoffset, int yoffset, int x, int y, int width, int height) =>
            (_glCopyTexSubImage2D ?? throw NotLoaded(nameof(_glCopyTexSubImage2D)))(target, level, xoffset, yoffset, x, y, width, height);

        public static void CopyTexSubImage3D(uint target, int level, int xoffset, int yoffset, int zoffset, int x, int y, int width, int height) =>
            (_glCopyTexSubImage3D ?? throw NotSupported(nameof(_glCopyTexSubImage3D), "Requires GLES3.0."))(target, level, xoffset, yoffset, zoffset, x, y, width, height);

        public static void DeleteBuffers(int n, ref uint buffers) =>
            (_glDeleteBuffers ?? throw NotLoaded(nameof(_glDeleteBuffers)))(n, ref buffers);

        public static void DeleteFramebuffers(int n, ref uint ids) =>
            (_glDeleteFramebuffers ?? throw NotLoaded(nameof(_glDeleteFramebuffers)))(n, ref ids);

        public static void DeleteQueries(int n, ref uint ids) =>
            (_glDeleteQueries ?? throw NotSupported(nameof(_glDeleteQueries), "Requires GLES3.0."))(n, ref ids);

        public static void DeleteRenderbuffers(int n, ref uint ids) =>
            (_glDeleteRenderbuffers ?? throw NotLoaded(nameof(_glDeleteRenderbuffers)))(n, ref ids);

        public static void DeleteSamplers(int n, ref uint samplers) =>
            (_glDeleteSamplers ?? throw NotSupported(nameof(_glDeleteSamplers), "Requires GLES3.0."))(n, ref samplers);

        public static void DeleteTextures(int n, ref uint textures) =>
            (_glDeleteTextures ?? throw NotLoaded(nameof(_glDeleteTextures)))(n, ref textures);

        public static void DeleteTransformFeedbacks(int n, ref uint ids) =>
            (_glDeleteTransformFeedbacks ?? throw NotSupported(nameof(_glDeleteTransformFeedbacks), "Requires GLES3.0."))(n, ref ids);

        public static void DeleteVertexArrays(int n, ref uint arrays) =>
            (_glDeleteVertexArrays ?? throw NotSupported(nameof(_glDeleteVertexArrays), "Requires GLES3.0."))(n, ref arrays);

        public static void DepthFunc(uint func) =>
            (_glDepthFunc ?? throw NotLoaded(nameof(_glDepthFunc)))(func);

        public static void DepthMask(byte flag) =>
            (_glDepthMask ?? throw NotLoaded(nameof(_glDepthMask)))(flag);

        public static void DepthRangef(float n, float f) =>
            (_glDepthRangef ?? throw NotLoaded(nameof(_glDepthRangef)))(n, f);

        public static void DetachShader(uint program, uint shader) =>
            (_glDetachShader ?? throw NotLoaded(nameof(_glDetachShader)))(program, shader);

        public static void DisableVertexAttribArray(uint index) =>
            (_glDisableVertexAttribArray ?? throw NotLoaded(nameof(_glDisableVertexAttribArray)))(index);

        public static void DrawRangeElements(uint mode, uint start, uint end, int count, uint type, IntPtr indices) =>
            (_glDrawRangeElements ?? throw NotSupported(nameof(_glDrawRangeElements), "Requires GLES3.0."))(mode, start, end, count, type, indices);

        public static void Flush() =>
            (_glFlush ?? throw NotLoaded(nameof(_glFlush)))();

        public static void FlushMappedBufferRange(uint target, nint offset, nint length) =>
            (_glFlushMappedBufferRange ?? throw NotSupported(nameof(_glFlushMappedBufferRange), "Requires GLES3.0."))(target, offset, length);

        public static void FramebufferTextureLayer(uint target, uint attachment, uint texture, int level, int layer) =>
            (_glFramebufferTextureLayer ?? throw NotSupported(nameof(_glFramebufferTextureLayer), "Requires GLES3.0."))(target, attachment, texture, level, layer);

        public static void FrontFace(uint mode) =>
            (_glFrontFace ?? throw NotLoaded(nameof(_glFrontFace)))(mode);

        public static void GenBuffers(int n, out uint buffers) =>
            (_glGenBuffers ?? throw NotLoaded(nameof(_glGenBuffers)))(n, out buffers);

        public static void GenFramebuffers(int n, out uint ids) =>
            (_glGenFramebuffers ?? throw NotLoaded(nameof(_glGenFramebuffers)))(n, out ids);

        public static void GenQueries(int n, out uint ids) =>
            (_glGenQueries ?? throw NotSupported(nameof(_glGenQueries), "Requires GLES3.0."))(n, out ids);

        public static void GenRenderbuffers(int n, out uint ids) =>
            (_glGenRenderbuffers ?? throw NotLoaded(nameof(_glGenRenderbuffers)))(n, out ids);

        public static void GenSamplers(int n, out uint samplers) =>
            (_glGenSamplers ?? throw NotSupported(nameof(_glGenSamplers), "Requires GLES3.0."))(n, out samplers);

        public static void GenTextures(int n, out uint textures) =>
            (_glGenTextures ?? throw NotLoaded(nameof(_glGenTextures)))(n, out textures);

        public static void GenTransformFeedbacks(int n, out uint ids) =>
            (_glGenTransformFeedbacks ?? throw NotSupported(nameof(_glGenTransformFeedbacks), "Requires GLES3.0."))(n, out ids);

        public static void GenVertexArrays(int n, out uint arrays) =>
            (_glGenVertexArrays ?? throw NotSupported(nameof(_glGenVertexArrays), "Requires GLES3.0."))(n, out arrays);

        public static void GetActiveAttrib(uint program, uint index, int bufSize, IntPtr length, IntPtr size, IntPtr type, IntPtr name) =>
            (_glGetActiveAttrib ?? throw NotLoaded(nameof(_glGetActiveAttrib)))(program, index, bufSize, length, size, type, name);

        public static void GetActiveUniform(uint program, uint index, int bufSize, IntPtr length, IntPtr size, IntPtr type, IntPtr name) =>
            (_glGetActiveUniform ?? throw NotLoaded(nameof(_glGetActiveUniform)))(program, index, bufSize, length, size, type, name);

        public static void GetActiveUniformBlockName(uint program, uint uniformBlockIndex, int bufSize, IntPtr length, IntPtr uniformBlockName) =>
            (_glGetActiveUniformBlockName ?? throw NotSupported(nameof(_glGetActiveUniformBlockName), "Requires GLES3.0."))(program, uniformBlockIndex, bufSize, length, uniformBlockName);

        public static void GetActiveUniformBlockiv(uint program, uint uniformBlockIndex, uint pname, IntPtr @params) =>
            (_glGetActiveUniformBlockiv ?? throw NotSupported(nameof(_glGetActiveUniformBlockiv), "Requires GLES3.0."))(program, uniformBlockIndex, pname, @params);

        public static void GetActiveUniformsiv(uint program, int uniformCount, IntPtr uniformIndices, uint pname, IntPtr @params) =>
            (_glGetActiveUniformsiv ?? throw NotSupported(nameof(_glGetActiveUniformsiv), "Requires GLES3.0."))(program, uniformCount, uniformIndices, pname, @params);

        public static void GetAttachedShaders(uint program, int maxCount, IntPtr count, IntPtr shaders) =>
            (_glGetAttachedShaders ?? throw NotLoaded(nameof(_glGetAttachedShaders)))(program, maxCount, count, shaders);

        public static void GetBooleanv(uint pname, out byte data) =>
            (_glGetBooleanv ?? throw NotLoaded(nameof(_glGetBooleanv)))(pname, out data);

        public static void GetBufferParameteri64v(uint target, uint pname, IntPtr @params) =>
            (_glGetBufferParameteri64v ?? throw NotSupported(nameof(_glGetBufferParameteri64v), "Requires GLES3.0."))(target, pname, @params);

        public static void GetBufferParameteriv(uint target, uint pname, IntPtr @params) =>
            (_glGetBufferParameteriv ?? throw NotLoaded(nameof(_glGetBufferParameteriv)))(target, pname, @params);

        public static void GetBufferPointerv(uint target, uint pname, IntPtr @params) =>
            (_glGetBufferPointerv ?? throw NotSupported(nameof(_glGetBufferPointerv), "Requires GLES3.0."))(target, pname, @params);

        public static void GetFloatv(uint pname, out float data) =>
            (_glGetFloatv ?? throw NotLoaded(nameof(_glGetFloatv)))(pname, out data);

        public static int GetFragDataLocation(uint program, string name) =>
            (_glGetFragDataLocation ?? throw NotSupported(nameof(_glGetFragDataLocation), "Requires GLES3.0."))(program, name);

        public static void GetFramebufferAttachmentParameteriv(uint target, uint attachment, uint pname, IntPtr @params) =>
            (_glGetFramebufferAttachmentParameteriv ?? throw NotLoaded(nameof(_glGetFramebufferAttachmentParameteriv)))(target, attachment, pname, @params);

        public static void GetInteger64i_v(uint target, uint index, IntPtr data) =>
            (_glGetInteger64i_v ?? throw NotSupported(nameof(_glGetInteger64i_v), "Requires GLES3.0."))(target, index, data);

        public static void GetInteger64v(uint pname, IntPtr data) =>
            (_glGetInteger64v ?? throw NotSupported(nameof(_glGetInteger64v), "Requires GLES3.0."))(pname, data);

        public static void GetIntegeri_v(uint target, uint index, IntPtr data) =>
            (_glGetIntegeri_v ?? throw NotSupported(nameof(_glGetIntegeri_v), "Requires GLES3.0."))(target, index, data);

        public static void GetIntegerv(uint pname, out int data) =>
            (_glGetIntegerv ?? throw NotLoaded(nameof(_glGetIntegerv)))(pname, out data);

        public static void GetInternalformativ(uint target, uint internalformat, uint pname, int count, IntPtr @params) =>
            (_glGetInternalformativ ?? throw NotSupported(nameof(_glGetInternalformativ), "Requires GLES3.0."))(target, internalformat, pname, count, @params);

        public static void GetProgramBinary(uint program, int bufSize, IntPtr length, IntPtr binaryFormat, IntPtr binary) =>
            (_glGetProgramBinary ?? throw NotSupported(nameof(_glGetProgramBinary), "Requires GLES3.0."))(program, bufSize, length, binaryFormat, binary);

        public static void GetProgramiv(uint program, uint pname, out int param) =>
            (_glGetProgramiv ?? throw NotLoaded(nameof(_glGetProgramiv)))(program, pname, out param);

        public static void GetQueryObjectuiv(uint id, uint pname, out uint paramsOut) =>
            (_glGetQueryObjectuiv ?? throw NotSupported(nameof(_glGetQueryObjectuiv), "Requires GLES3.0."))(id, pname, out paramsOut);

        public static void GetQueryiv(uint target, uint pname, IntPtr @params) =>
            (_glGetQueryiv ?? throw NotSupported(nameof(_glGetQueryiv), "Requires GLES3.0."))(target, pname, @params);

        public static void GetRenderbufferParameteriv(uint target, uint pname, IntPtr @params) =>
            (_glGetRenderbufferParameteriv ?? throw NotLoaded(nameof(_glGetRenderbufferParameteriv)))(target, pname, @params);

        public static void GetSamplerParameterfv(uint sampler, uint pname, IntPtr @params) =>
            (_glGetSamplerParameterfv ?? throw NotSupported(nameof(_glGetSamplerParameterfv), "Requires GLES3.0."))(sampler, pname, @params);

        public static void GetSamplerParameteriv(uint sampler, uint pname, IntPtr @params) =>
            (_glGetSamplerParameteriv ?? throw NotSupported(nameof(_glGetSamplerParameteriv), "Requires GLES3.0."))(sampler, pname, @params);

        public static void GetShaderPrecisionFormat(uint shadertype, uint precisiontype, IntPtr range, IntPtr precision) =>
            (_glGetShaderPrecisionFormat ?? throw NotLoaded(nameof(_glGetShaderPrecisionFormat)))(shadertype, precisiontype, range, precision);

        public static void GetShaderSource(uint shader, int bufSize, IntPtr length, IntPtr source) =>
            (_glGetShaderSource ?? throw NotLoaded(nameof(_glGetShaderSource)))(shader, bufSize, length, source);

        public static void GetShaderiv(uint shader, uint pname, out int param) =>
            (_glGetShaderiv ?? throw NotLoaded(nameof(_glGetShaderiv)))(shader, pname, out param);

        public static IntPtr GetString(uint name) =>
            (_glGetString ?? throw NotLoaded(nameof(_glGetString)))(name);

        public static IntPtr GetStringi(uint name, uint index) =>
            (_glGetStringi ?? throw NotSupported(nameof(_glGetStringi), "Requires GLES3.0."))(name, index);

        public static void GetSynciv(IntPtr sync, uint pname, int bufSize, out int length, IntPtr values) =>
            (_glGetSynciv ?? throw NotSupported(nameof(_glGetSynciv), "Requires GLES3.0."))(sync, pname, bufSize, out length, values);

        public static void GetTexParameterfv(uint target, uint pname, IntPtr @params) =>
            (_glGetTexParameterfv ?? throw NotLoaded(nameof(_glGetTexParameterfv)))(target, pname, @params);

        public static void GetTexParameteriv(uint target, uint pname, IntPtr @params) =>
            (_glGetTexParameteriv ?? throw NotLoaded(nameof(_glGetTexParameteriv)))(target, pname, @params);

        public static void GetTransformFeedbackVarying(uint program, uint index, int bufSize, out int length, out int size, out uint type, IntPtr name) =>
            (_glGetTransformFeedbackVarying ?? throw NotSupported(nameof(_glGetTransformFeedbackVarying), "Requires GLES3.0."))(program, index, bufSize, out length, out size, out type, name);

        public static uint GetUniformBlockIndex(uint program, string uniformBlockName) =>
            (_glGetUniformBlockIndex ?? throw NotSupported(nameof(_glGetUniformBlockIndex), "Requires GLES3.0."))(program, uniformBlockName);

        public static void GetUniformIndices(uint program, int uniformCount, IntPtr uniformNames, IntPtr uniformIndices) =>
            (_glGetUniformIndices ?? throw NotSupported(nameof(_glGetUniformIndices), "Requires GLES3.0."))(program, uniformCount, uniformNames, uniformIndices);

        public static void GetUniformfv(uint program, int location, IntPtr @params) =>
            (_glGetUniformfv ?? throw NotLoaded(nameof(_glGetUniformfv)))(program, location, @params);

        public static void GetUniformiv(uint program, int location, IntPtr @params) =>
            (_glGetUniformiv ?? throw NotLoaded(nameof(_glGetUniformiv)))(program, location, @params);

        public static void GetUniformuiv(uint program, int location, IntPtr @params) =>
            (_glGetUniformuiv ?? throw NotSupported(nameof(_glGetUniformuiv), "Requires GLES3.0."))(program, location, @params);

        public static void GetVertexAttribIiv(uint index, uint pname, IntPtr @params) =>
            (_glGetVertexAttribIiv ?? throw NotSupported(nameof(_glGetVertexAttribIiv), "Requires GLES3.0."))(index, pname, @params);

        public static void GetVertexAttribIuiv(uint index, uint pname, IntPtr @params) =>
            (_glGetVertexAttribIuiv ?? throw NotSupported(nameof(_glGetVertexAttribIuiv), "Requires GLES3.0."))(index, pname, @params);

        public static void GetVertexAttribPointerv(uint index, uint pname, IntPtr pointer) =>
            (_glGetVertexAttribPointerv ?? throw NotLoaded(nameof(_glGetVertexAttribPointerv)))(index, pname, pointer);

        public static void GetVertexAttribfv(uint index, uint pname, IntPtr @params) =>
            (_glGetVertexAttribfv ?? throw NotLoaded(nameof(_glGetVertexAttribfv)))(index, pname, @params);

        public static void GetVertexAttribiv(uint index, uint pname, IntPtr @params) =>
            (_glGetVertexAttribiv ?? throw NotLoaded(nameof(_glGetVertexAttribiv)))(index, pname, @params);

        public static void Hint(uint target, uint mode) =>
            (_glHint ?? throw NotLoaded(nameof(_glHint)))(target, mode);

        public static void InvalidateFramebuffer(uint target, int numAttachments, IntPtr attachments) =>
            (_glInvalidateFramebuffer ?? throw NotSupported(nameof(_glInvalidateFramebuffer), "Requires GLES3.0."))(target, numAttachments, attachments);

        public static void InvalidateSubFramebuffer(uint target, int numAttachments, IntPtr attachments, int x, int y, int width, int height) =>
            (_glInvalidateSubFramebuffer ?? throw NotSupported(nameof(_glInvalidateSubFramebuffer), "Requires GLES3.0."))(target, numAttachments, attachments, x, y, width, height);

        public static byte IsBuffer(uint buffer) =>
            (_glIsBuffer ?? throw NotLoaded(nameof(_glIsBuffer)))(buffer);

        public static byte IsEnabled(uint cap) =>
            (_glIsEnabled ?? throw NotLoaded(nameof(_glIsEnabled)))(cap);

        public static byte IsFramebuffer(uint framebuffer) =>
            (_glIsFramebuffer ?? throw NotLoaded(nameof(_glIsFramebuffer)))(framebuffer);

        public static byte IsProgram(uint program) =>
            (_glIsProgram ?? throw NotLoaded(nameof(_glIsProgram)))(program);

        public static byte IsQuery(uint id) =>
            (_glIsQuery ?? throw NotSupported(nameof(_glIsQuery), "Requires GLES3.0."))(id);

        public static byte IsRenderbuffer(uint renderbuffer) =>
            (_glIsRenderbuffer ?? throw NotLoaded(nameof(_glIsRenderbuffer)))(renderbuffer);

        public static byte IsSampler(uint sampler) =>
            (_glIsSampler ?? throw NotSupported(nameof(_glIsSampler), "Requires GLES3.0."))(sampler);

        public static byte IsShader(uint shader) =>
            (_glIsShader ?? throw NotLoaded(nameof(_glIsShader)))(shader);

        public static byte IsTexture(uint texture) =>
            (_glIsTexture ?? throw NotLoaded(nameof(_glIsTexture)))(texture);

        public static byte IsTransformFeedback(uint id) =>
            (_glIsTransformFeedback ?? throw NotSupported(nameof(_glIsTransformFeedback), "Requires GLES3.0."))(id);

        public static byte IsVertexArray(uint array) =>
            (_glIsVertexArray ?? throw NotSupported(nameof(_glIsVertexArray), "Requires GLES3.0."))(array);

        public static void LineWidth(float width) =>
            (_glLineWidth ?? throw NotLoaded(nameof(_glLineWidth)))(width);

        public static IntPtr MapBufferRange(uint target, nint offset, nint length, uint access) =>
            (_glMapBufferRange ?? throw NotSupported(nameof(_glMapBufferRange), "Requires GLES3.0."))(target, offset, length, access);

        public static void PolygonOffset(float factor, float units) =>
            (_glPolygonOffset ?? throw NotLoaded(nameof(_glPolygonOffset)))(factor, units);

        public static void ProgramBinary(uint program, uint binaryFormat, IntPtr binary, int length) =>
            (_glProgramBinary ?? throw NotSupported(nameof(_glProgramBinary), "Requires GLES3.0."))(program, binaryFormat, binary, length);

        public static void ProgramParameteri(uint program, uint pname, int value) =>
            (_glProgramParameteri ?? throw NotSupported(nameof(_glProgramParameteri), "Requires GLES3.0."))(program, pname, value);

        public static void ReadBuffer(uint src) =>
            (_glReadBuffer ?? throw NotSupported(nameof(_glReadBuffer), "Requires GLES3.0."))(src);

        public static void ReleaseShaderCompiler() =>
            (_glReleaseShaderCompiler ?? throw NotLoaded(nameof(_glReleaseShaderCompiler)))();

        public static void RenderbufferStorageMultisample(uint target, int samples, uint internalformat, int width, int height) =>
            (_glRenderbufferStorageMultisample ?? throw NotSupported(nameof(_glRenderbufferStorageMultisample), "Requires GLES3.0."))(target, samples, internalformat, width, height);

        public static void SampleCoverage(float value, byte invert) =>
            (_glSampleCoverage ?? throw NotLoaded(nameof(_glSampleCoverage)))(value, invert);

        public static void SamplerParameterfv(uint sampler, uint pname, IntPtr param) =>
            (_glSamplerParameterfv ?? throw NotSupported(nameof(_glSamplerParameterfv), "Requires GLES3.0."))(sampler, pname, param);

        public static void SamplerParameteriv(uint sampler, uint pname, IntPtr param) =>
            (_glSamplerParameteriv ?? throw NotSupported(nameof(_glSamplerParameteriv), "Requires GLES3.0."))(sampler, pname, param);

        public static void Scissor(int x, int y, int width, int height) =>
            (_glScissor ?? throw NotLoaded(nameof(_glScissor)))(x, y, width, height);

        public static void ShaderBinary(int count, IntPtr shaders, uint binaryFormat, IntPtr binary, int length) =>
            (_glShaderBinary ?? throw NotLoaded(nameof(_glShaderBinary)))(count, shaders, binaryFormat, binary, length);

        public static void StencilFuncSeparate(uint face, uint func, int @ref, uint mask) =>
            (_glStencilFuncSeparate ?? throw NotLoaded(nameof(_glStencilFuncSeparate)))(face, func, @ref, mask);

        public static void StencilMaskSeparate(uint face, uint mask) =>
            (_glStencilMaskSeparate ?? throw NotLoaded(nameof(_glStencilMaskSeparate)))(face, mask);

        public static void StencilOpSeparate(uint face, uint sfail, uint dpfail, uint dppass) =>
            (_glStencilOpSeparate ?? throw NotLoaded(nameof(_glStencilOpSeparate)))(face, sfail, dpfail, dppass);

        public static void TexParameterfv(uint target, uint pname, IntPtr @params) =>
            (_glTexParameterfv ?? throw NotLoaded(nameof(_glTexParameterfv)))(target, pname, @params);

        public static void TexParameteriv(uint target, uint pname, IntPtr @params) =>
            (_glTexParameteriv ?? throw NotLoaded(nameof(_glTexParameteriv)))(target, pname, @params);

        public static void TexStorage2D(uint target, int levels, uint internalformat, int width, int height) =>
            (_glTexStorage2D ?? throw NotSupported(nameof(_glTexStorage2D), "Requires GLES3.0."))(target, levels, internalformat, width, height);

        public static void TexStorage3D(uint target, int levels, uint internalformat, int width, int height, int depth) =>
            (_glTexStorage3D ?? throw NotSupported(nameof(_glTexStorage3D), "Requires GLES3.0."))(target, levels, internalformat, width, height, depth);

        public static void TexSubImage2D(uint target, int level, int xoffset, int yoffset, int width, int height, uint format, uint type, IntPtr pixels) =>
            (_glTexSubImage2D ?? throw NotLoaded(nameof(_glTexSubImage2D)))(target, level, xoffset, yoffset, width, height, format, type, pixels);

        public static void TexSubImage3D(uint target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, uint format, uint type, IntPtr pixels) =>
            (_glTexSubImage3D ?? throw NotSupported(nameof(_glTexSubImage3D), "Requires GLES3.0."))(target, level, xoffset, yoffset, zoffset, width, height, depth, format, type, pixels);

        public static void Uniform1fv(int location, int count, IntPtr value) =>
            (_glUniform1fv ?? throw NotLoaded(nameof(_glUniform1fv)))(location, count, value);

        public static void Uniform1iv(int location, int count, IntPtr value) =>
            (_glUniform1iv ?? throw NotLoaded(nameof(_glUniform1iv)))(location, count, value);

        public static void Uniform1ui(int location, uint v0) =>
            (_glUniform1ui ?? throw NotSupported(nameof(_glUniform1ui), "Requires GLES3.0."))(location, v0);

        public static void Uniform1uiv(int location, int count, IntPtr value) =>
            (_glUniform1uiv ?? throw NotSupported(nameof(_glUniform1uiv), "Requires GLES3.0."))(location, count, value);

        public static void Uniform2f(int location, float v0, float v1) =>
            (_glUniform2f ?? throw NotLoaded(nameof(_glUniform2f)))(location, v0, v1);

        public static void Uniform2fv(int location, int count, IntPtr value) =>
            (_glUniform2fv ?? throw NotLoaded(nameof(_glUniform2fv)))(location, count, value);

        public static void Uniform2i(int location, int v0, int v1) =>
            (_glUniform2i ?? throw NotLoaded(nameof(_glUniform2i)))(location, v0, v1);

        public static void Uniform2iv(int location, int count, IntPtr value) =>
            (_glUniform2iv ?? throw NotLoaded(nameof(_glUniform2iv)))(location, count, value);

        public static void Uniform2ui(int location, uint v0, uint v1) =>
            (_glUniform2ui ?? throw NotSupported(nameof(_glUniform2ui), "Requires GLES3.0."))(location, v0, v1);

        public static void Uniform2uiv(int location, int count, IntPtr value) =>
            (_glUniform2uiv ?? throw NotSupported(nameof(_glUniform2uiv), "Requires GLES3.0."))(location, count, value);

        public static void Uniform3fv(int location, int count, IntPtr value) =>
            (_glUniform3fv ?? throw NotLoaded(nameof(_glUniform3fv)))(location, count, value);

        public static void Uniform3i(int location, int v0, int v1, int v2) =>
            (_glUniform3i ?? throw NotLoaded(nameof(_glUniform3i)))(location, v0, v1, v2);

        public static void Uniform3iv(int location, int count, IntPtr value) =>
            (_glUniform3iv ?? throw NotLoaded(nameof(_glUniform3iv)))(location, count, value);

        public static void Uniform3ui(int location, uint v0, uint v1, uint v2) =>
            (_glUniform3ui ?? throw NotSupported(nameof(_glUniform3ui), "Requires GLES3.0."))(location, v0, v1, v2);

        public static void Uniform3uiv(int location, int count, IntPtr value) =>
            (_glUniform3uiv ?? throw NotSupported(nameof(_glUniform3uiv), "Requires GLES3.0."))(location, count, value);

        public static void Uniform4fv(int location, int count, IntPtr value) =>
            (_glUniform4fv ?? throw NotLoaded(nameof(_glUniform4fv)))(location, count, value);

        public static void Uniform4i(int location, int v0, int v1, int v2, int v3) =>
            (_glUniform4i ?? throw NotLoaded(nameof(_glUniform4i)))(location, v0, v1, v2, v3);

        public static void Uniform4iv(int location, int count, IntPtr value) =>
            (_glUniform4iv ?? throw NotLoaded(nameof(_glUniform4iv)))(location, count, value);

        public static void Uniform4ui(int location, uint v0, uint v1, uint v2, uint v3) =>
            (_glUniform4ui ?? throw NotSupported(nameof(_glUniform4ui), "Requires GLES3.0."))(location, v0, v1, v2, v3);

        public static void Uniform4uiv(int location, int count, IntPtr value) =>
            (_glUniform4uiv ?? throw NotSupported(nameof(_glUniform4uiv), "Requires GLES3.0."))(location, count, value);

        public static void UniformBlockBinding(uint program, uint uniformBlockIndex, uint uniformBlockBinding) =>
            (_glUniformBlockBinding ?? throw NotSupported(nameof(_glUniformBlockBinding), "Requires GLES3.0."))(program, uniformBlockIndex, uniformBlockBinding);

        public static void UniformMatrix2fv(int location, int count, byte transpose, IntPtr value) =>
            (_glUniformMatrix2fv ?? throw NotLoaded(nameof(_glUniformMatrix2fv)))(location, count, transpose, value);

        public static void UniformMatrix2x3fv(int location, int count, byte transpose, IntPtr value) =>
            (_glUniformMatrix2x3fv ?? throw NotSupported(nameof(_glUniformMatrix2x3fv), "Requires GLES3.0."))(location, count, transpose, value);

        public static void UniformMatrix2x4fv(int location, int count, byte transpose, IntPtr value) =>
            (_glUniformMatrix2x4fv ?? throw NotSupported(nameof(_glUniformMatrix2x4fv), "Requires GLES3.0."))(location, count, transpose, value);

        public static void UniformMatrix3fv(int location, int count, byte transpose, IntPtr value) =>
            (_glUniformMatrix3fv ?? throw NotLoaded(nameof(_glUniformMatrix3fv)))(location, count, transpose, value);

        public static void UniformMatrix3x2fv(int location, int count, byte transpose, IntPtr value) =>
            (_glUniformMatrix3x2fv ?? throw NotSupported(nameof(_glUniformMatrix3x2fv), "Requires GLES3.0."))(location, count, transpose, value);

        public static void UniformMatrix3x4fv(int location, int count, byte transpose, IntPtr value) =>
            (_glUniformMatrix3x4fv ?? throw NotSupported(nameof(_glUniformMatrix3x4fv), "Requires GLES3.0."))(location, count, transpose, value);

        public static void UniformMatrix4x2fv(int location, int count, byte transpose, IntPtr value) =>
            (_glUniformMatrix4x2fv ?? throw NotSupported(nameof(_glUniformMatrix4x2fv), "Requires GLES3.0."))(location, count, transpose, value);

        public static void UniformMatrix4x3fv(int location, int count, byte transpose, IntPtr value) =>
            (_glUniformMatrix4x3fv ?? throw NotSupported(nameof(_glUniformMatrix4x3fv), "Requires GLES3.0."))(location, count, transpose, value);

        public static byte UnmapBuffer(uint target) =>
            (_glUnmapBuffer ?? throw NotSupported(nameof(_glUnmapBuffer), "Requires GLES3.0."))(target);

        public static void ValidateProgram(uint program) =>
            (_glValidateProgram ?? throw NotLoaded(nameof(_glValidateProgram)))(program);

        public static void VertexAttrib1f(uint index, float x) =>
            (_glVertexAttrib1f ?? throw NotLoaded(nameof(_glVertexAttrib1f)))(index, x);

        public static void VertexAttrib1fv(uint index, IntPtr v) =>
            (_glVertexAttrib1fv ?? throw NotLoaded(nameof(_glVertexAttrib1fv)))(index, v);

        public static void VertexAttrib2f(uint index, float x, float y) =>
            (_glVertexAttrib2f ?? throw NotLoaded(nameof(_glVertexAttrib2f)))(index, x, y);

        public static void VertexAttrib2fv(uint index, IntPtr v) =>
            (_glVertexAttrib2fv ?? throw NotLoaded(nameof(_glVertexAttrib2fv)))(index, v);

        public static void VertexAttrib3f(uint index, float x, float y, float z) =>
            (_glVertexAttrib3f ?? throw NotLoaded(nameof(_glVertexAttrib3f)))(index, x, y, z);

        public static void VertexAttrib3fv(uint index, IntPtr v) =>
            (_glVertexAttrib3fv ?? throw NotLoaded(nameof(_glVertexAttrib3fv)))(index, v);

        public static void VertexAttrib4f(uint index, float x, float y, float z, float w) =>
            (_glVertexAttrib4f ?? throw NotLoaded(nameof(_glVertexAttrib4f)))(index, x, y, z, w);

        public static void VertexAttrib4fv(uint index, IntPtr v) =>
            (_glVertexAttrib4fv ?? throw NotLoaded(nameof(_glVertexAttrib4fv)))(index, v);

        public static void VertexAttribI4i(uint index, int x, int y, int z, int w) =>
            (_glVertexAttribI4i ?? throw NotSupported(nameof(_glVertexAttribI4i), "Requires GLES3.0."))(index, x, y, z, w);

        public static void VertexAttribI4iv(uint index, IntPtr v) =>
            (_glVertexAttribI4iv ?? throw NotSupported(nameof(_glVertexAttribI4iv), "Requires GLES3.0."))(index, v);

        public static void VertexAttribI4ui(uint index, uint x, uint y, uint z, uint w) =>
            (_glVertexAttribI4ui ?? throw NotSupported(nameof(_glVertexAttribI4ui), "Requires GLES3.0."))(index, x, y, z, w);

        public static void VertexAttribI4uiv(uint index, IntPtr v) =>
            (_glVertexAttribI4uiv ?? throw NotSupported(nameof(_glVertexAttribI4uiv), "Requires GLES3.0."))(index, v);

        public static void VertexAttribIPointer(uint index, int size, uint type, int stride, IntPtr pointer) =>
            (_glVertexAttribIPointer ?? throw NotSupported(nameof(_glVertexAttribIPointer), "Requires GLES3.0."))(index, size, type, stride, pointer);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glBindAttribLocation_t(uint program, uint index, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glBlendColor_t(float red, float green, float blue, float alpha);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glBlendEquation_t(uint mode);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glBlendEquationSeparate_t(uint modeRGB, uint modeAlpha);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glBlendFuncSeparate_t(uint sfactorRGB, uint dfactorRGB, uint sfactorAlpha, uint dfactorAlpha);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glBufferSubData_t(uint target, nint offset, nint size, IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glClearBufferfi_t(uint buffer, int drawbuffer, float depth, int stencil);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glClearBufferfv_t(uint buffer, int drawbuffer, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glClearBufferiv_t(uint buffer, int drawbuffer, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glClearBufferuiv_t(uint buffer, int drawbuffer, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glColorMask_t(byte red, byte green, byte blue, byte alpha);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glCompressedTexImage2D_t(uint target, int level, uint internalformat, int width, int height, int border, int imageSize, IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glCompressedTexImage3D_t(uint target, int level, uint internalformat, int width, int height, int depth, int border, int imageSize, IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glCompressedTexSubImage2D_t(uint target, int level, int xoffset, int yoffset, int width, int height, uint format, int imageSize, IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glCompressedTexSubImage3D_t(uint target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, uint format, int imageSize, IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glCopyBufferSubData_t(uint readTarget, uint writeTarget, nint readOffset, nint writeOffset, nint size);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glCopyTexImage2D_t(uint target, int level, uint internalformat, int x, int y, int width, int height, int border);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glCopyTexSubImage2D_t(uint target, int level, int xoffset, int yoffset, int x, int y, int width, int height);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glCopyTexSubImage3D_t(uint target, int level, int xoffset, int yoffset, int zoffset, int x, int y, int width, int height);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glDepthFunc_t(uint func);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glDepthMask_t(byte flag);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glDepthRangef_t(float n, float f);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glDetachShader_t(uint program, uint shader);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glDisableVertexAttribArray_t(uint index);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glDrawArraysInstanced_t(uint mode, int first, int count, int instancecount);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glDrawElementsInstanced_t(uint mode, int count, uint type, IntPtr indices, int instancecount);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glDrawRangeElements_t(uint mode, uint start, uint end, int count, uint type, IntPtr indices);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glFlush_t();

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glFlushMappedBufferRange_t(uint target, nint offset, nint length);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glFramebufferTextureLayer_t(uint target, uint attachment, uint texture, int level, int layer);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glFrontFace_t(uint mode);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetActiveAttrib_t(uint program, uint index, int bufSize, IntPtr length, IntPtr size, IntPtr type, IntPtr name);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetActiveUniform_t(uint program, uint index, int bufSize, IntPtr length, IntPtr size, IntPtr type, IntPtr name);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetActiveUniformBlockName_t(uint program, uint uniformBlockIndex, int bufSize, IntPtr length, IntPtr uniformBlockName);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetActiveUniformBlockiv_t(uint program, uint uniformBlockIndex, uint pname, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetActiveUniformsiv_t(uint program, int uniformCount, IntPtr uniformIndices, uint pname, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetAttachedShaders_t(uint program, int maxCount, IntPtr count, IntPtr shaders);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetBufferParameteri64v_t(uint target, uint pname, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetBufferParameteriv_t(uint target, uint pname, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetBufferPointerv_t(uint target, uint pname, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate int glGetFragDataLocation_t(uint program, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetFramebufferAttachmentParameteriv_t(uint target, uint attachment, uint pname, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetInteger64i_v_t(uint target, uint index, IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetInteger64v_t(uint pname, IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetIntegeri_v_t(uint target, uint index, IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetInternalformativ_t(uint target, uint internalformat, uint pname, int count, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetProgramBinary_t(uint program, int bufSize, IntPtr length, IntPtr binaryFormat, IntPtr binary);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetQueryiv_t(uint target, uint pname, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetRenderbufferParameteriv_t(uint target, uint pname, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetSamplerParameterfv_t(uint sampler, uint pname, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetSamplerParameteriv_t(uint sampler, uint pname, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetShaderPrecisionFormat_t(uint shadertype, uint precisiontype, IntPtr range, IntPtr precision);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetShaderSource_t(uint shader, int bufSize, IntPtr length, IntPtr source);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr glGetString_t(uint name);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr glGetStringi_t(uint name, uint index);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetTexParameterfv_t(uint target, uint pname, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetTexParameteriv_t(uint target, uint pname, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate uint glGetUniformBlockIndex_t(uint program, [MarshalAs(UnmanagedType.LPUTF8Str)] string uniformBlockName);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetUniformIndices_t(uint program, int uniformCount, IntPtr uniformNames, IntPtr uniformIndices);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetUniformfv_t(uint program, int location, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetUniformiv_t(uint program, int location, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetUniformuiv_t(uint program, int location, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetVertexAttribIiv_t(uint index, uint pname, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetVertexAttribIuiv_t(uint index, uint pname, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetVertexAttribPointerv_t(uint index, uint pname, IntPtr pointer);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetVertexAttribfv_t(uint index, uint pname, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glGetVertexAttribiv_t(uint index, uint pname, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glHint_t(uint target, uint mode);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glInvalidateFramebuffer_t(uint target, int numAttachments, IntPtr attachments);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glInvalidateSubFramebuffer_t(uint target, int numAttachments, IntPtr attachments, int x, int y, int width, int height);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate byte glIsBuffer_t(uint buffer);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate byte glIsEnabled_t(uint cap);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate byte glIsFramebuffer_t(uint framebuffer);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate byte glIsProgram_t(uint program);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate byte glIsQuery_t(uint id);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate byte glIsRenderbuffer_t(uint renderbuffer);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate byte glIsSampler_t(uint sampler);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate byte glIsShader_t(uint shader);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate byte glIsTexture_t(uint texture);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate byte glIsTransformFeedback_t(uint id);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate byte glIsVertexArray_t(uint array);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glLineWidth_t(float width);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate IntPtr glMapBufferRange_t(uint target, nint offset, nint length, uint access);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glPolygonOffset_t(float factor, float units);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glProgramBinary_t(uint program, uint binaryFormat, IntPtr binary, int length);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glProgramParameteri_t(uint program, uint pname, int value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glReadBuffer_t(uint src);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glReleaseShaderCompiler_t();

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glRenderbufferStorageMultisample_t(uint target, int samples, uint internalformat, int width, int height);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glSampleCoverage_t(float value, byte invert);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glSamplerParameterfv_t(uint sampler, uint pname, IntPtr param);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glSamplerParameteriv_t(uint sampler, uint pname, IntPtr param);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glScissor_t(int x, int y, int width, int height);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glShaderBinary_t(int count, IntPtr shaders, uint binaryFormat, IntPtr binary, int length);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glStencilFuncSeparate_t(uint face, uint func, int @ref, uint mask);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glStencilMaskSeparate_t(uint face, uint mask);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glStencilOpSeparate_t(uint face, uint sfail, uint dpfail, uint dppass);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glTexParameterfv_t(uint target, uint pname, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glTexParameteriv_t(uint target, uint pname, IntPtr @params);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glTexStorage2D_t(uint target, int levels, uint internalformat, int width, int height);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glTexStorage3D_t(uint target, int levels, uint internalformat, int width, int height, int depth);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glTexSubImage2D_t(uint target, int level, int xoffset, int yoffset, int width, int height, uint format, uint type, IntPtr pixels);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glTexSubImage3D_t(uint target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, uint format, uint type, IntPtr pixels);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniform1fv_t(int location, int count, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniform1iv_t(int location, int count, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniform1ui_t(int location, uint v0);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniform1uiv_t(int location, int count, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniform2f_t(int location, float v0, float v1);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniform2fv_t(int location, int count, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniform2i_t(int location, int v0, int v1);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniform2iv_t(int location, int count, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniform2ui_t(int location, uint v0, uint v1);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniform2uiv_t(int location, int count, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniform3fv_t(int location, int count, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniform3i_t(int location, int v0, int v1, int v2);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniform3iv_t(int location, int count, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniform3ui_t(int location, uint v0, uint v1, uint v2);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniform3uiv_t(int location, int count, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniform4fv_t(int location, int count, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniform4i_t(int location, int v0, int v1, int v2, int v3);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniform4iv_t(int location, int count, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniform4ui_t(int location, uint v0, uint v1, uint v2, uint v3);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniform4uiv_t(int location, int count, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniformBlockBinding_t(uint program, uint uniformBlockIndex, uint uniformBlockBinding);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniformMatrix2fv_t(int location, int count, byte transpose, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniformMatrix2x3fv_t(int location, int count, byte transpose, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniformMatrix2x4fv_t(int location, int count, byte transpose, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniformMatrix3fv_t(int location, int count, byte transpose, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniformMatrix3x2fv_t(int location, int count, byte transpose, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniformMatrix3x4fv_t(int location, int count, byte transpose, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniformMatrix4x2fv_t(int location, int count, byte transpose, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glUniformMatrix4x3fv_t(int location, int count, byte transpose, IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate byte glUnmapBuffer_t(uint target);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glValidateProgram_t(uint program);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glVertexAttrib1f_t(uint index, float x);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glVertexAttrib1fv_t(uint index, IntPtr v);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glVertexAttrib2f_t(uint index, float x, float y);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glVertexAttrib2fv_t(uint index, IntPtr v);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glVertexAttrib3f_t(uint index, float x, float y, float z);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glVertexAttrib3fv_t(uint index, IntPtr v);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glVertexAttrib4f_t(uint index, float x, float y, float z, float w);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glVertexAttrib4fv_t(uint index, IntPtr v);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glVertexAttribDivisor_t(uint index, uint divisor);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glVertexAttribI4i_t(uint index, int x, int y, int z, int w);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glVertexAttribI4iv_t(uint index, IntPtr v);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glVertexAttribI4ui_t(uint index, uint x, uint y, uint z, uint w);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glVertexAttribI4uiv_t(uint index, IntPtr v);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate void glVertexAttribIPointer_t(uint index, int size, uint type, int stride, IntPtr pointer);

    }
}
