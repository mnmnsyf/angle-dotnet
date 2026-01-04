using AngleBridge.Sdk.HotReload;
using AngleBridge.Sdk.Runtime;
using AngleBridge.Utilities;
using System;
using System.Diagnostics;

namespace AngleBridge.Utilities.WinForms
{
    public delegate void WinFormsFrameCallback(AngleGl gl, int width, int height, float dt, double totalTime);

    public static class WinFormsHotReloadHost
    {
        public static int Run(WinFormsHostOptions options, WinFormsFrameCallback? onFrame = null)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(options.PluginPath))
                throw new ArgumentException("Plugin path must be set.", nameof(options));

            try
            {
                using var window = new WinFormsSampleWindow(
                    options.Title,
                    options.Width,
                    options.Height,
                    options.Visible);

                var deviceOptions = new AngleDeviceOptions
                {
                    GlesMajor = options.GlesMajor,
                    Backend = options.Backend,
                    DeviceType = options.DeviceType,
                    SwapInterval = options.SwapInterval,
                };

                using var device = AngleDevice.CreateWindow(window.NativeHandle, deviceOptions);
                if (options.EnableDebug)
                    device.TryEnableDebugOutput(LogDebugMessage, options.DebugSynchronous);

                using var reloader = new RendererHotReloader(options.PluginPath);
                reloader.Start();

                var timer = Stopwatch.StartNew();
                double prevTime = 0.0;
                long frameCount = 0;
                bool running = true;
                Exception? lastReloadError = null;

                while (running)
                {
                    var elapsed = timer.Elapsed.TotalSeconds;
                    var dt = (float)(elapsed - prevTime);

                    window.PumpEvents();
                    while (window.TryDequeueEvent(out var ev))
                    {
                        if (ev.Type == SampleEventType.Closed)
                            running = false;
                    }

                    if (!running)
                        break;

                    reloader.Tick();
                    var reloadError = reloader.LastError;
                    if (reloadError != null && !ReferenceEquals(reloadError, lastReloadError))
                    {
                        lastReloadError = reloadError;
                        Console.Error.WriteLine(reloadError);
                    }

                    var gl = device.Gl;
                    var width = window.Width;
                    var height = window.Height;

                    onFrame?.Invoke(gl, width, height, dt, elapsed);
                    var renderer = reloader.Current;
                    renderer.Draw(gl, width, height);
                    if (renderer is IRendererWithPresent presenter)
                        presenter.Present(device, width, height);
                    else
                        device.Present();

                    prevTime = elapsed;
                    frameCount++;

                    if (options.Frames > 0 && frameCount >= options.Frames)
                        running = false;
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return -1;
            }
        }

        private static void LogDebugMessage(string message)
        {
            Console.Error.WriteLine(message);
        }
    }
}
