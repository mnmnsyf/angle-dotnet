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
            Console.WriteLine("Demos: --demo=3 | --demo=4 | --demo=5 | --demo=6 | --demo=7 | --demo=8 | --demo=9 | --demo=10 | --demo=11 | --demo=12 | --demo=13 | --demo=14 | --demo=15 | --demo=16 | --demo=17 | --demo=18");
            Console.WriteLine("       --demo=triangle | --demo=hello_triangle | --demo=simple_vertex_shader | --demo=stencil_operations | --demo=tri_fan_microbench | --demo=simple_texture_2d | --demo=multi_texture | --demo=mip_map_2d | --demo=simple_texture_cubemap | --demo=particle_system | --demo=tex_redef_microbench | --demo=multiple_draw_buffers | --demo=simple_instancing | --demo=multiview | --demo=obj_model | --demo=post_sub_buffer");
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
            Console.WriteLine(" 10 / mip_map_2d            -> demos/Demo10_MipMap2D.Plugin");
            Console.WriteLine(" 11 / simple_texture_cubemap-> demos/Demo11_SimpleTextureCubemap.Plugin");
            Console.WriteLine(" 12 / particle_system       -> demos/Demo12_ParticleSystem.Plugin");
            Console.WriteLine(" 13 / tex_redef_microbench  -> demos/Demo13_TexRedefMicrobench.Plugin");
            Console.WriteLine(" 14 / multiple_draw_buffers -> demos/Demo14_MultipleDrawBuffers.Plugin");
            Console.WriteLine(" 15 / simple_instancing     -> demos/Demo15_SimpleInstancing.Plugin");
            Console.WriteLine(" 16 / multiview             -> demos/Demo16_Multiview.Plugin");
            Console.WriteLine(" 17 / obj_model             -> demos/Demo17_ObjModel.Plugin");
            Console.WriteLine(" 18 / post_sub_buffer       -> demos/Demo18_PostSubBuffer.Plugin");
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

            if (d == "10" || d == "mip_map_2d" || d == "mipmap2d" || d == "demo10")
            {
                options.Title = "MipMap2D";
                options.Width = 800;
                options.Height = 600;
                options.Visible = true;
                options.GlesMajor = 3;
                options.PluginPath = ResolvePluginPath("Demo10_MipMap2D.Plugin.dll", "Demo10_MipMap2D.Plugin");
                return options;
            }

            if (d == "11" || d == "simple_texture_cubemap" || d == "cubemap" || d == "demo11")
            {
                options.Title = "SimpleTextureCubemap";
                options.Width = 640;
                options.Height = 480;
                options.Visible = true;
                options.GlesMajor = 2;
                options.PluginPath = ResolvePluginPath("Demo11_SimpleTextureCubemap.Plugin.dll", "Demo11_SimpleTextureCubemap.Plugin");
                return options;
            }

            if (d == "12" || d == "particle_system" || d == "particles" || d == "demo12")
            {
                options.Title = "ParticleSystem";
                options.Width = 640;
                options.Height = 480;
                options.Visible = true;
                options.GlesMajor = 2;
                options.PluginPath = ResolvePluginPath("Demo12_ParticleSystem.Plugin.dll", "Demo12_ParticleSystem.Plugin");
                return options;
            }

            if (d == "13" || d == "tex_redef_microbench" || d == "texredef" || d == "demo13")
            {
                options.Title = "TexRedefMicrobench";
                options.Width = 1280;
                options.Height = 1280;
                options.Visible = true;
                options.GlesMajor = 2;
                options.PluginPath = ResolvePluginPath("Demo13_TexRedefMicrobench.Plugin.dll", "Demo13_TexRedefMicrobench.Plugin");
                return options;
            }

            if (d == "14" || d == "multiple_draw_buffers" || d == "mdb" || d == "demo14")
            {
                options.Title = "MultipleDrawBuffers";
                options.Width = 640;
                options.Height = 480;
                options.Visible = true;
                options.GlesMajor = 2;
                options.PluginPath = ResolvePluginPath("Demo14_MultipleDrawBuffers.Plugin.dll", "Demo14_MultipleDrawBuffers.Plugin");
                return options;
            }

            if (d == "15" || d == "simple_instancing" || d == "instancing" || d == "demo15")
            {
                options.Title = "SimpleInstancing";
                options.Width = 640;
                options.Height = 480;
                options.Visible = true;
                options.GlesMajor = 2;
                options.PluginPath = ResolvePluginPath("Demo15_SimpleInstancing.Plugin.dll", "Demo15_SimpleInstancing.Plugin");
                return options;
            }

            if (d == "16" || d == "multiview" || d == "mv" || d == "demo16")
            {
                options.Title = "Multiview";
                options.Width = 960;
                options.Height = 480;
                options.Visible = true;
                options.GlesMajor = 3;
                options.PluginPath = ResolvePluginPath("Demo16_Multiview.Plugin.dll", "Demo16_Multiview.Plugin");
                return options;
            }

            if (d == "17" || d == "obj_model" || d == "obj" || d == "demo17")
            {
                options.Title = "ObjModel";
                options.Width = 800;
                options.Height = 600;
                options.Visible = true;
                options.GlesMajor = 3;
                options.PluginPath = ResolvePluginPath("Demo17_ObjModel.Plugin.dll", "Demo17_ObjModel.Plugin");
                return options;
            }

            if (d == "18" || d == "post_sub_buffer" || d == "postsubbuffer" || d == "demo18")
            {
                options.Title = "PostSubBuffer";
                options.Width = 640;
                options.Height = 480;
                options.Visible = true;
                options.GlesMajor = 3;
                options.PluginPath = ResolvePluginPath("Demo18_PostSubBuffer.Plugin.dll", "Demo18_PostSubBuffer.Plugin");
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
