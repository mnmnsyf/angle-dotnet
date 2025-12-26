using System;

namespace AngleBridge.Utilities
{
    public interface ISampleWindow : IDisposable
    {
        IntPtr NativeHandle { get; }
        int Width { get; }
        int Height { get; }
        int X { get; }
        int Y { get; }
        bool Visible { get; }

        void SetTitle(string title);
        void SetPosition(int x, int y);
        void SetSize(int width, int height);
        void SetVisible(bool visible);

        void PumpEvents();
        bool TryDequeueEvent(out SampleEvent ev);
    }
}

