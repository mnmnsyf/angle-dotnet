using AngleBridge.Sdk.Runtime;
using System;
using System.Globalization;

namespace AngleBridge.Utilities.WinForms
{
    public static class WinFormsHostCli
    {
        public static WinFormsHostOptions Parse(string[] args, WinFormsHostOptions defaults, string appName)
        {
            if (defaults == null) throw new ArgumentNullException(nameof(defaults));
            if (string.IsNullOrWhiteSpace(appName)) throw new ArgumentException("App name must be set.", nameof(appName));

            var options = defaults.Clone();
            foreach (var arg in args)
            {
                if (arg.StartsWith("--plugin=", StringComparison.OrdinalIgnoreCase))
                {
                    options.PluginPath = arg.Substring("--plugin=".Length);
                    continue;
                }

                if (arg.StartsWith("--title=", StringComparison.OrdinalIgnoreCase))
                {
                    options.Title = arg.Substring("--title=".Length);
                    continue;
                }

                if (arg.StartsWith("--use-angle=", StringComparison.OrdinalIgnoreCase))
                {
                    var value = arg.Substring("--use-angle=".Length);
                    ApplyUseAngle(options, value);
                    continue;
                }

                if (arg.StartsWith("--backend=", StringComparison.OrdinalIgnoreCase))
                {
                    options.Backend = ParseBackend(arg.Substring("--backend=".Length));
                    continue;
                }

                if (arg.StartsWith("--device=", StringComparison.OrdinalIgnoreCase))
                {
                    options.DeviceType = ParseDeviceType(arg.Substring("--device=".Length));
                    continue;
                }

                if (arg.StartsWith("--swap-interval=", StringComparison.OrdinalIgnoreCase))
                {
                    options.SwapInterval = ParseInt(arg, "--swap-interval=");
                    continue;
                }

                if (arg.StartsWith("--width=", StringComparison.OrdinalIgnoreCase))
                {
                    options.Width = ParseInt(arg, "--width=");
                    continue;
                }

                if (arg.StartsWith("--height=", StringComparison.OrdinalIgnoreCase))
                {
                    options.Height = ParseInt(arg, "--height=");
                    continue;
                }

                if (arg.StartsWith("--frames=", StringComparison.OrdinalIgnoreCase))
                {
                    options.Frames = ParseInt(arg, "--frames=");
                    continue;
                }

                if (arg.StartsWith("--gles-major=", StringComparison.OrdinalIgnoreCase))
                {
                    options.GlesMajor = ParseInt(arg, "--gles-major=");
                    continue;
                }

                if (arg.Equals("--show", StringComparison.OrdinalIgnoreCase) ||
                    arg.Equals("--visible", StringComparison.OrdinalIgnoreCase))
                {
                    options.Visible = true;
                    continue;
                }

                if (arg.Equals("--debug", StringComparison.OrdinalIgnoreCase))
                {
                    options.EnableDebug = true;
                    options.DebugSynchronous = true;
                    continue;
                }

                if (arg.Equals("--debug-async", StringComparison.OrdinalIgnoreCase))
                {
                    options.EnableDebug = true;
                    options.DebugSynchronous = false;
                    continue;
                }

                // Demo-specific passthroughs (e.g., Demo8 texture options). We don't need to store them;
                // plugins read Environment.GetCommandLineArgs. Accept here to avoid "Unknown argument" errors.
                if (arg.StartsWith("--tex=", StringComparison.OrdinalIgnoreCase) ||
                    arg.StartsWith("--tex-min=", StringComparison.OrdinalIgnoreCase) ||
                    arg.StartsWith("--tex-mag=", StringComparison.OrdinalIgnoreCase) ||
                    arg.StartsWith("--tex-wrap-s=", StringComparison.OrdinalIgnoreCase) ||
                    arg.StartsWith("--tex-wrap-t=", StringComparison.OrdinalIgnoreCase) ||
                    arg.StartsWith("--tex-wrap-preset=", StringComparison.OrdinalIgnoreCase) ||
                    arg.StartsWith("--tex0=", StringComparison.OrdinalIgnoreCase) ||
                    arg.StartsWith("--tex1=", StringComparison.OrdinalIgnoreCase) ||
                    arg.StartsWith("--tex-base=", StringComparison.OrdinalIgnoreCase) ||
                    arg.StartsWith("--tex-light=", StringComparison.OrdinalIgnoreCase) ||
                    arg.Equals("--tex-mipmap", StringComparison.OrdinalIgnoreCase) ||
                    arg.Equals("--tex-no-mipmap", StringComparison.OrdinalIgnoreCase) ||
                    arg.Equals("--tex-wrap-uv", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (arg.Equals("--use-gl=native", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                throw new ArgumentException("Unknown argument: " + arg);
            }

            if (string.IsNullOrWhiteSpace(options.PluginPath))
                throw new ArgumentException("Plugin path is required. Use --plugin=<path>.");

            return options;
        }

        public static void PrintUsage(string appName)
        {
            Console.WriteLine("Usage: " + appName + " [options]");
            Console.WriteLine("  --plugin=<path>           Path to plugin DLL (required)");
            Console.WriteLine("  --title=<text>            Window title");
            Console.WriteLine("  --use-angle=<backend>     e.g. d3d11, d3d9, vulkan, opengl, opengles, metal, null, warp, reference, swiftshader");
            Console.WriteLine("  --backend=<backend>       Same as --use-angle without device shortcuts");
            Console.WriteLine("  --device=<device>         hardware, warp, reference, swiftshader, null");
            Console.WriteLine("  --swap-interval=<n>       0 disables vsync (default: 0)");
            Console.WriteLine("  --width=<n>               (default: 640)");
            Console.WriteLine("  --height=<n>              (default: 480)");
            Console.WriteLine("  --frames=<n>              Exit after N frames (default: 0)");
            Console.WriteLine("  --gles-major=<n>          2 or 3 (default: 3)");
            Console.WriteLine("  --show                    Show window (default: hidden)");
            Console.WriteLine("  --debug                   Enable GL_KHR_debug output (synchronous)");
            Console.WriteLine("  --debug-async             Enable GL_KHR_debug output (async)");
            Console.WriteLine("  [demo-specific passthroughs] --tex=... --tex0/--tex1=... --tex-min=... --tex-mag=... --tex-wrap-s/t=... --tex-wrap-preset=... --tex-wrap-uv --tex-mipmap/--tex-no-mipmap");
        }

        private static void ApplyUseAngle(WinFormsHostOptions options, string value)
        {
            var v = value.Trim().ToLowerInvariant();
            switch (v)
            {
                case "default":
                    options.Backend = AngleBackend.Default;
                    options.DeviceType = AngleDeviceType.Default;
                    return;
                case "d3d11":
                    options.Backend = AngleBackend.D3D11;
                    options.DeviceType = AngleDeviceType.Default;
                    return;
                case "d3d9":
                    options.Backend = AngleBackend.D3D9;
                    options.DeviceType = AngleDeviceType.Default;
                    return;
                case "vulkan":
                    options.Backend = AngleBackend.Vulkan;
                    options.DeviceType = AngleDeviceType.Default;
                    return;
                case "opengl":
                    options.Backend = AngleBackend.OpenGL;
                    options.DeviceType = AngleDeviceType.Default;
                    return;
                case "opengles":
                    options.Backend = AngleBackend.OpenGles;
                    options.DeviceType = AngleDeviceType.Default;
                    return;
                case "metal":
                    options.Backend = AngleBackend.Metal;
                    options.DeviceType = AngleDeviceType.Default;
                    return;
                case "null":
                    options.Backend = AngleBackend.Null;
                    options.DeviceType = AngleDeviceType.Default;
                    return;
                case "warp":
                    options.Backend = AngleBackend.D3D11;
                    options.DeviceType = AngleDeviceType.Warp;
                    return;
                case "reference":
                    options.Backend = AngleBackend.D3D11;
                    options.DeviceType = AngleDeviceType.Reference;
                    return;
                case "swiftshader":
                    options.Backend = AngleBackend.Vulkan;
                    options.DeviceType = AngleDeviceType.SwiftShader;
                    return;
                default:
                    throw new ArgumentException("Unknown --use-angle value: " + value);
            }
        }

        private static AngleBackend ParseBackend(string value)
        {
            var v = value.Trim().ToLowerInvariant();
            switch (v)
            {
                case "default": return AngleBackend.Default;
                case "d3d11": return AngleBackend.D3D11;
                case "d3d9": return AngleBackend.D3D9;
                case "vulkan": return AngleBackend.Vulkan;
                case "opengl": return AngleBackend.OpenGL;
                case "opengles": return AngleBackend.OpenGles;
                case "metal": return AngleBackend.Metal;
                case "null": return AngleBackend.Null;
                default: throw new ArgumentException("Unknown backend: " + value);
            }
        }

        private static AngleDeviceType ParseDeviceType(string value)
        {
            var v = value.Trim().ToLowerInvariant();
            switch (v)
            {
                case "default": return AngleDeviceType.Default;
                case "hardware": return AngleDeviceType.Hardware;
                case "warp": return AngleDeviceType.Warp;
                case "reference": return AngleDeviceType.Reference;
                case "swiftshader": return AngleDeviceType.SwiftShader;
                case "null": return AngleDeviceType.Null;
                default: throw new ArgumentException("Unknown device type: " + value);
            }
        }

        private static int ParseInt(string arg, string prefix)
        {
            var value = arg.Substring(prefix.Length);
            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
                throw new ArgumentException("Invalid value for " + prefix + value);
            return result;
        }
    }
}
