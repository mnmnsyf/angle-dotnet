using AngleBridge.Sdk.Runtime;
using System;
using System.Diagnostics;

namespace AngleBridge.Utilities
{
    public abstract class SampleApplication : IDisposable
    {
        private readonly ISampleWindow _window;
        private readonly AngleDeviceOptions _deviceOptions;
        private readonly bool _enableDebug;
        private readonly bool _debugSynchronous;

        private AngleDevice? _device;
        private bool _running;
        private long _frameCount;
        private readonly Stopwatch _timer = new Stopwatch();

        protected SampleApplication(ISampleWindow window, AngleDeviceOptions options, bool enableDebug, bool debugSynchronous)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
            _deviceOptions = options ?? throw new ArgumentNullException(nameof(options));
            _enableDebug = enableDebug;
            _debugSynchronous = debugSynchronous;
        }

        protected AngleGl Gl
        {
            get
            {
                if (_device == null) throw new InvalidOperationException("Device not initialized.");
                return _device.Gl;
            }
        }

        protected ISampleWindow Window { get { return _window; } }
        protected int ClientWidth { get { return _window.Width; } }
        protected int ClientHeight { get { return _window.Height; } }

        public int Run(int frameLimit)
        {
            if (_running) throw new InvalidOperationException("Already running.");
            try
            {
                _device = AngleDevice.CreateWindow(_window.NativeHandle, _deviceOptions);
                if (_enableDebug && _device != null)
                {
                    _device.TryEnableDebugOutput(LogDebugMessage, _debugSynchronous);
                }

                if (!Initialize())
                    return -1;

                _running = true;
                _timer.Restart();
                double prevTime = 0.0;

                while (_running)
                {
                    double elapsed = _timer.Elapsed.TotalSeconds;
                    double dt = elapsed - prevTime;

                    Step((float)dt, elapsed);

                    _window.PumpEvents();
                    ProcessEvents();

                    if (!_running)
                        break;

                    Draw();
                    Swap();

                    prevTime = elapsed;
                    _frameCount++;

                    if (frameLimit > 0 && _frameCount >= frameLimit)
                        Exit();
                }

                Destroy();
                return 0;
            }
            finally
            {
                Cleanup();
            }
        }

        public void Exit()
        {
            _running = false;
        }

        public void Dispose()
        {
            Cleanup();
            GC.SuppressFinalize(this);
        }

        protected virtual bool Initialize() { return true; }
        protected virtual void Destroy() { }
        protected virtual void Step(float dt, double totalTime) { }
        protected virtual void OnKeyDown(SampleKeyEvent keyEvent) { }
        protected virtual void OnKeyUp(SampleKeyEvent keyEvent) { }
        protected virtual void OnMouseMove(SampleMouseEvent mouseEvent) { }
        protected virtual void OnMouseDown(SampleMouseEvent mouseEvent) { }
        protected virtual void OnMouseUp(SampleMouseEvent mouseEvent) { }
        protected virtual void OnMouseWheel(SampleMouseEvent mouseEvent) { }
        protected virtual void OnResize(int width, int height) { }
        protected virtual void OnFocusGained() { }
        protected virtual void OnFocusLost() { }
        protected abstract void Draw();

        protected virtual void Swap()
        {
            if (_device == null) return;
            _device.Present();
        }

        private void ProcessEvents()
        {
            while (_window.TryDequeueEvent(out var ev))
            {
                switch (ev.Type)
                {
                    case SampleEventType.Closed:
                        Exit();
                        break;
                    case SampleEventType.KeyPressed:
                        OnKeyDown(ev.Key);
                        break;
                    case SampleEventType.KeyReleased:
                        OnKeyUp(ev.Key);
                        break;
                    case SampleEventType.MouseMoved:
                        OnMouseMove(ev.Mouse);
                        break;
                    case SampleEventType.MouseButtonDown:
                        OnMouseDown(ev.Mouse);
                        break;
                    case SampleEventType.MouseButtonUp:
                        OnMouseUp(ev.Mouse);
                        break;
                    case SampleEventType.MouseWheel:
                        OnMouseWheel(ev.Mouse);
                        break;
                    case SampleEventType.Resized:
                        OnResize(ev.Width, ev.Height);
                        break;
                    case SampleEventType.FocusGained:
                        OnFocusGained();
                        break;
                    case SampleEventType.FocusLost:
                        OnFocusLost();
                        break;
                }
            }
        }

        private static void LogDebugMessage(string message)
        {
            Console.Error.WriteLine(message);
        }

        private void Cleanup()
        {
            _running = false;

            var device = _device;
            _device = null;
            if (device != null)
                device.Dispose();

            _window.Dispose();
        }
    }
}

