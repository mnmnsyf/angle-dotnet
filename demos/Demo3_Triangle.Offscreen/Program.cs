using AngleBridge.Managed.GLES;
using AngleBridge.Managed.Runtime;
using Demo3_Triangle.Plugin;
using System;
using System.IO;
using System.Text;

namespace Demo3_Triangle.Offscreen;

internal static class Program
{
    private static int Main(string[] args)
    {
        var width = 256;
        var height = 256;
        var outPath = "demo3_csharp.ppm";

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--out" when i + 1 < args.Length:
                    outPath = args[++i];
                    break;
                case "--w" when i + 1 < args.Length:
                    width = int.Parse(args[++i]);
                    break;
                case "--h" when i + 1 < args.Length:
                    height = int.Parse(args[++i]);
                    break;
            }
        }

        if (width <= 0 || height <= 0)
        {
            Console.Error.WriteLine("Width/height must be positive.");
            return 1;
        }

        using var ctx = AngleContext.CreateOffscreen(3, width, height);
        GlesLoader.Load();

        using var renderer = new TriangleRenderer();
        renderer.Draw(width, height);
        Gles.Finish();

        var rgba = new byte[checked(width * height * 4)];
        Gles.ReadPixels(0, 0, width, height, rgba);

        EnsureOutputDirectory(outPath);
        WritePpm(outPath, width, height, rgba);
        Console.WriteLine($"Wrote: {outPath}");
        return 0;
    }

    private static void EnsureOutputDirectory(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (string.IsNullOrWhiteSpace(dir)) return;
        Directory.CreateDirectory(dir);
    }

    private static void WritePpm(string path, int width, int height, byte[] rgba)
    {
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        var header = $"P6\n{width} {height}\n255\n";
        var headerBytes = Encoding.ASCII.GetBytes(header);
        fs.Write(headerBytes, 0, headerBytes.Length);

        for (var y = height - 1; y >= 0; y--)
        {
            var row = y * width * 4;
            for (var x = 0; x < width; x++)
            {
                var i = row + x * 4;
                fs.WriteByte(rgba[i + 0]);
                fs.WriteByte(rgba[i + 1]);
                fs.WriteByte(rgba[i + 2]);
            }
        }
    }
}
