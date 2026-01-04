using AngleBridge.Managed.GLES;
using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    partial class AngleGl
    {
        public readonly struct TexturesApi
        {
            private readonly AngleGl _gl;

            internal TexturesApi(AngleGl gl)
            {
                _gl = gl;
            }

            public GlTextureHandle GenTexture()
            {
                _gl.Guard();
                return new GlTextureHandle(I32(Gles.GenTexture()));
            }

            public AngleTexture CreateTextureObject()
            {
                _gl.Guard();
                var handle = new GlTextureHandle(I32(Gles.GenTexture()));
                return new AngleTexture(_gl, handle);
            }

            public void DeleteTexture(GlTextureHandle texture)
            {
                _gl.Guard();
                if (texture.IsNull) return;
                Gles.DeleteTexture(U32(texture.Value));
            }

            public void BindTexture(TextureTarget target, GlTextureHandle texture)
            {
                _gl.Guard();
                Gles.BindTexture(U32((int)target), U32(texture.Value));
            }

            public void BindTexture(TextureTarget target, AngleTexture texture)
            {
                _gl.Guard();
                _gl.ValidateTexture(texture);
                Gles.BindTexture(U32((int)target), U32(texture.Handle.Value));
            }

            public void ActiveTexture(TextureUnit texture)
            {
                _gl.Guard();
                Gles.ActiveTexture(U32((int)texture));
            }

            public void TexParameteri(TextureTarget target, TextureParameterName pname, int param)
            {
                _gl.Guard();
                Gles.TexParameteri(U32((int)target), U32((int)pname), param);
            }

            public void TexParameterf(TextureTarget target, TextureParameterName pname, float param)
            {
                _gl.Guard();
                Gles.TexParameterf(U32((int)target), U32((int)pname), param);
            }

            public void PixelStore(PixelStoreParameter pname, int param)
            {
                _gl.Guard();
                Gles.PixelStorei(U32((int)pname), param);
            }

            public void TexImage2D(TextureTarget target, int level, TextureInternalFormat internalFormat, int width, int height, int border, PixelFormat format, PixelType type, byte[]? pixels)
            {
                _gl.Guard();
                Gles.TexImage2D(U32((int)target), level, (int)internalFormat, width, height, border, U32((int)format), U32((int)type), pixels);
            }

            public void GenerateMipmap(TextureTarget target)
            {
                _gl.Guard();
                Gles.GenerateMipmap(U32((int)target));
            }

            public void TexImage3D(TextureTarget target, int level, TextureInternalFormat internalFormat, int width, int height, int depth, int border, PixelFormat format, PixelType type, byte[]? pixels)
            {
                _gl.Guard();
                Gles.TexImage3D(U32((int)target), level, (int)internalFormat, width, height, depth, border, U32((int)format), U32((int)type), pixels);
            }
        }
    }
}
