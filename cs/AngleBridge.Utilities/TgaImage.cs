using System;
using System.IO;

namespace AngleBridge.Utilities
{
    public sealed class TgaImage
    {
        private TgaImage(int width, int height, byte[] rgba)
        {
            Width = width;
            Height = height;
            Rgba = rgba;
        }

        public int Width { get; }
        public int Height { get; }
        public byte[] Rgba { get; }

        public static bool TryLoad(string path, out TgaImage? image)
        {
            image = null;
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return false;

            try
            {
                using var stream = File.OpenRead(path);
                using var reader = new BinaryReader(stream);

                var idSize = reader.ReadByte();
                var mapType = reader.ReadByte();
                var imageType = reader.ReadByte();
                var paletteStart = reader.ReadUInt16();
                var paletteSize = reader.ReadUInt16();
                var paletteEntryDepth = reader.ReadByte();
                var x = reader.ReadUInt16();
                var y = reader.ReadUInt16();
                var width = reader.ReadUInt16();
                var height = reader.ReadUInt16();
                var colorDepth = reader.ReadByte();
                var descriptor = reader.ReadByte();

                if (mapType != 0)
                    return false;
                if (imageType != 2)
                    return false;
                if (width == 0 || height == 0)
                    return false;

                _ = paletteStart;
                _ = paletteSize;
                _ = paletteEntryDepth;
                _ = x;
                _ = y;

                if (idSize > 0)
                    reader.ReadBytes(idSize);

                var bytesPerPixel = colorDepth / 8;
                if (bytesPerPixel != 3 && bytesPerPixel != 4)
                    return false;

                var pixelCount = checked(width * height);
                var src = reader.ReadBytes(checked(pixelCount * bytesPerPixel));
                if (src.Length != pixelCount * bytesPerPixel)
                    return false;

                var inverted = (descriptor & 0x20) != 0;
                var dst = new byte[pixelCount * 4];

                for (int row = 0; row < height; row++)
                {
                    var srcRow = inverted ? (height - 1 - row) : row;
                    var srcRowOffset = srcRow * width * bytesPerPixel;
                    var dstRowOffset = row * width * 4;

                    for (int col = 0; col < width; col++)
                    {
                        var srcIndex = srcRowOffset + col * bytesPerPixel;
                        var dstIndex = dstRowOffset + col * 4;

                        var b = src[srcIndex + 0];
                        var g = src[srcIndex + 1];
                        var r = bytesPerPixel > 2 ? src[srcIndex + 2] : (byte)0;
                        var a = bytesPerPixel > 3 ? src[srcIndex + 3] : (byte)255;

                        dst[dstIndex + 0] = r;
                        dst[dstIndex + 1] = g;
                        dst[dstIndex + 2] = b;
                        dst[dstIndex + 3] = a;
                    }
                }

                image = new TgaImage(width, height, dst);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

