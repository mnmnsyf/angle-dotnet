using AngleBridge.Sdk.Interop;
using AngleBridge.Sdk.Runtime;
using System;

namespace AngleBridge.Utilities
{
    public static class TextureUtils
    {
        public static GlTextureHandle CreateSimpleTexture2D(AngleGl gl)
        {
            if (gl == null) throw new ArgumentNullException(nameof(gl));

            gl.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            var texture = gl.GenTexture();
            gl.BindTexture(TextureTarget.Texture2D, texture);

            const int width = 2;
            const int height = 2;
            var pixels = new byte[]
            {
                255, 0, 0,
                0, 255, 0,
                0, 0, 255,
                255, 255, 0,
            };

            gl.TexImage2D(TextureTarget.Texture2D, 0, TextureInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, pixels);
            gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            return texture;
        }

        public static GlTextureHandle CreateSimpleTextureCubemap(AngleGl gl)
        {
            if (gl == null) throw new ArgumentNullException(nameof(gl));

            var texture = gl.GenTexture();
            gl.BindTexture(TextureTarget.TextureCubeMap, texture);

            var pixels = new byte[][]
            {
                new byte[] { 255, 0, 0 },
                new byte[] { 0, 255, 0 },
                new byte[] { 0, 0, 255 },
                new byte[] { 255, 255, 0 },
                new byte[] { 255, 0, 255 },
                new byte[] { 255, 255, 255 },
            };

            for (var i = 0; i < 6; i++)
            {
                var face = (TextureTarget)((int)TextureTarget.TextureCubeMapPositiveX + i);
                gl.TexImage2D(face, 0, TextureInternalFormat.Rgb, 1, 1, 0, PixelFormat.Rgb, PixelType.UnsignedByte, pixels[i]);
            }

            gl.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            gl.TexParameteri(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            return texture;
        }

        public static GlTextureHandle CreateMipMappedTexture2D(AngleGl gl)
        {
            if (gl == null) throw new ArgumentNullException(nameof(gl));

            const int width = 256;
            const int height = 256;
            var pixels = new byte[width * height * 3];

            const int checkerSize = 8;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    byte rColor;
                    byte bColor;

                    if ((x / checkerSize) % 2 == 0)
                    {
                        rColor = (byte)(255 * ((y / checkerSize) % 2));
                        bColor = (byte)(255 * (1 - ((y / checkerSize) % 2)));
                    }
                    else
                    {
                        bColor = (byte)(255 * ((y / checkerSize) % 2));
                        rColor = (byte)(255 * (1 - ((y / checkerSize) % 2)));
                    }

                    var index = (y * width + x) * 3;
                    pixels[index + 0] = rColor;
                    pixels[index + 1] = 0;
                    pixels[index + 2] = bColor;
                }
            }

            var texture = gl.GenTexture();
            gl.BindTexture(TextureTarget.Texture2D, texture);

            gl.TexImage2D(TextureTarget.Texture2D, 0, TextureInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, pixels);
            gl.GenerateMipmap(TextureTarget.Texture2D);

            gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
            gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return texture;
        }

        public static GlTextureHandle LoadTextureFromTga(AngleGl gl, TgaImage image)
        {
            if (gl == null) throw new ArgumentNullException(nameof(gl));
            if (image == null) throw new ArgumentNullException(nameof(image));

            var texture = gl.GenTexture();
            gl.BindTexture(TextureTarget.Texture2D, texture);

            gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            gl.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            gl.TexImage2D(TextureTarget.Texture2D, 0, TextureInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Rgba);
            gl.GenerateMipmap(TextureTarget.Texture2D);

            return texture;
        }

        public static GlTextureHandle LoadTextureFromTgaFile(AngleGl gl, string path)
        {
            if (!TgaImage.TryLoad(path, out var image) || image == null)
                return default;

            return LoadTextureFromTga(gl, image);
        }
    }
}

