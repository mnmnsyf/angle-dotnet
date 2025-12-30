using AngleBridge.Utilities.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DemoHost.WinForms
{
    internal static class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            string? demo = null;
            var forwardArgs = new List<string>();
            foreach (var arg in args)
            {
                if (arg.StartsWith("--demo=", StringComparison.OrdinalIgnoreCase))
                {
                    demo = arg.Substring("--demo=".Length);
                    continue;
                }

                if (arg.Equals("--list-demos", StringComparison.OrdinalIgnoreCase))
                {
                    PrintDemos();
                    return 0;
                }

                forwardArgs.Add(arg);
            }

            try
            {
                var defaults = CreateDefaults(demo);
                var options = WinFormsHostCli.Parse(forwardArgs.ToArray(), defaults, "DemoHost.WinForms");
                return WinFormsHotReloadHost.Run(options);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                PrintUsage();
                return -1;
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage: DemoHost.WinForms --demo=<demo> [options]");
            Console.WriteLine("   or: DemoHost.WinForms --plugin=<path> [options]");
            Console.WriteLine("Demos: --demo=3 | --demo=4 | --demo=5 | --demo=6 | --demo=7 | --demo=8 | --demo=9");
            Console.WriteLine("       --demo=triangle | --demo=hello_triangle | --demo=simple_vertex_shader | --demo=stencil_operations | --demo=tri_fan_microbench | --demo=simple_texture_2d | --demo=multi_texture");
            Console.WriteLine("       --list-demos");
            Console.WriteLine();
            WinFormsHostCli.PrintUsage("DemoHost.WinForms");
        }

        private static void PrintDemos()
        {
            Console.WriteLine("Available demos:");
            Console.WriteLine("  3 / triangle              -> demos/Demo3_Triangle.Plugin");
            Console.WriteLine("  4 / hello_triangle        -> demos/Demo4_HelloTriangle.Plugin");
            Console.WriteLine("  5 / simple_vertex_shader  -> demos/Demo5_SimpleVertexShader.Plugin");
            Console.WriteLine("  6 / stencil_operations    -> demos/Demo6_StencilOperations.Plugin");
            Console.WriteLine("  7 / tri_fan_microbench    -> demos/Demo7_TriFanMicrobench.Plugin");
            Console.WriteLine("  8 / simple_texture_2d     -> demos/Demo8_SimpleTexture2D.Plugin");
            Console.WriteLine("  9 / multi_texture         -> demos/Demo9_MultiTexture.Plugin");
        }

        private static WinFormsHostOptions CreateDefaults(string? demo)
        {
            var options = new WinFormsHostOptions
            {
                Title = "ANGLE Demo Host",
                Width = 640,
                Height = 480,
                Frames = 0,
                Visible = false,
                SwapInterval = 0,
                GlesMajor = 3,
            };

            if (string.IsNullOrWhiteSpace(demo))
                return options;

            var d = demo.Trim().ToLowerInvariant();
            if (d == "3" || d == "triangle" || d == "demo3")
            {
                options.Title = "Triangle";
                options.GlesMajor = 3;
                options.PluginPath = ResolvePluginPath("Demo3_Triangle.Plugin.dll", "Demo3_Triangle.Plugin");
                return options;
            }

            if (d == "4" || d == "hello_triangle" || d == "hello" || d == "demo4")
            {
                options.Title = "HelloTriangle";
                options.GlesMajor = 2;
                options.PluginPath = ResolvePluginPath("Demo4_HelloTriangle.Plugin.dll", "Demo4_HelloTriangle.Plugin");
                return options;
            }

            if (d == "5" || d == "simple_vertex_shader" || d == "simple" || d == "demo5")
            {
                options.Title = "SimpleVertexShader";
                options.GlesMajor = 3;
                options.PluginPath = ResolvePluginPath("Demo5_SimpleVertexShader.Plugin.dll", "Demo5_SimpleVertexShader.Plugin");
                return options;
            }

            if (d == "6" || d == "stencil_operations" || d == "stencil" || d == "demo6")
            {
                options.Title = "StencilOperations";
                options.GlesMajor = 3;
                options.PluginPath = ResolvePluginPath("Demo6_StencilOperations.Plugin.dll", "Demo6_StencilOperations.Plugin");
                return options;
            }

            if (d == "7" || d == "tri_fan_microbench" || d == "trifan" || d == "demo7")
            {
                options.Title = "TriFanMicrobench";
                options.Width = 1280;
                options.Height = 1280;
                options.Visible = true;
                options.GlesMajor = 2;
                options.PluginPath = ResolvePluginPath("Demo7_TriFanMicrobench.Plugin.dll", "Demo7_TriFanMicrobench.Plugin");
                return options;
            }

            if (d == "8" || d == "simple_texture_2d" || d == "texture2d" || d == "demo8")
            {
                options.Title = "SimpleTexture2D";
                options.Width = 640;
                options.Height = 480;
                options.Visible = true;
                options.GlesMajor = 3;
                options.PluginPath = ResolvePluginPath("Demo8_SimpleTexture2D.Plugin.dll", "Demo8_SimpleTexture2D.Plugin");
                return options;
            }

            if (d == "9" || d == "multi_texture" || d == "multitexture" || d == "demo9")
            {
                options.Title = "MultiTexture";
                options.Width = 640;
                options.Height = 480;
                options.Visible = true;
                options.GlesMajor = 3;
                options.PluginPath = ResolvePluginPath("Demo9_MultiTexture.Plugin.dll", "Demo9_MultiTexture.Plugin");
                return options;
            }

            throw new ArgumentException("Unknown demo: " + demo);
        }

        private static string ResolvePluginPath(string pluginDllName, string pluginProjectDirName)
        {
            var baseDir = AppContext.BaseDirectory;
#if DEBUG
            const string configuration = "Debug";
#else
            const string configuration = "Release";
#endif

            var current = new DirectoryInfo(baseDir);
            for (var i = 0; i < 8 && current != null; i++)
            {
                var candidate = Path.Combine(
                    current.FullName,
                    "demos",
                    pluginProjectDirName,
                    "bin",
                    configuration,
                    "net9.0",
                    pluginDllName);
                if (File.Exists(candidate)) return candidate;
                current = current.Parent;
            }

            return Path.Combine(baseDir, "plugins", pluginDllName);
        }
    }
}
