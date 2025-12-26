using System;

namespace AngleBridge.Utilities
{
    public enum SampleEventType
    {
        Closed = 0,
        KeyPressed = 1,
        KeyReleased = 2,
        MouseMoved = 3,
        MouseButtonDown = 4,
        MouseButtonUp = 5,
        MouseWheel = 6,
        Resized = 7,
        FocusGained = 8,
        FocusLost = 9,
    }

    public enum SampleMouseButton
    {
        Unknown = 0,
        Left = 1,
        Right = 2,
        Middle = 3,
        XButton1 = 4,
        XButton2 = 5,
    }

    public readonly struct SampleKeyEvent
    {
        public SampleKeyEvent(int keyCode, bool alt, bool control, bool shift)
        {
            KeyCode = keyCode;
            Alt = alt;
            Control = control;
            Shift = shift;
        }

        public int KeyCode { get; }
        public bool Alt { get; }
        public bool Control { get; }
        public bool Shift { get; }
    }

    public readonly struct SampleMouseEvent
    {
        public SampleMouseEvent(SampleMouseButton button, int x, int y, int delta)
        {
            Button = button;
            X = x;
            Y = y;
            Delta = delta;
        }

        public SampleMouseButton Button { get; }
        public int X { get; }
        public int Y { get; }
        public int Delta { get; }
    }

    public readonly struct SampleEvent
    {
        private SampleEvent(SampleEventType type, SampleKeyEvent key, SampleMouseEvent mouse, int width, int height)
        {
            Type = type;
            Key = key;
            Mouse = mouse;
            Width = width;
            Height = height;
        }

        public SampleEventType Type { get; }
        public SampleKeyEvent Key { get; }
        public SampleMouseEvent Mouse { get; }
        public int Width { get; }
        public int Height { get; }

        public static SampleEvent Closed()
        {
            return new SampleEvent(SampleEventType.Closed, default, default, 0, 0);
        }

        public static SampleEvent KeyDown(int keyCode, bool alt, bool control, bool shift)
        {
            return new SampleEvent(SampleEventType.KeyPressed, new SampleKeyEvent(keyCode, alt, control, shift), default, 0, 0);
        }

        public static SampleEvent KeyUp(int keyCode, bool alt, bool control, bool shift)
        {
            return new SampleEvent(SampleEventType.KeyReleased, new SampleKeyEvent(keyCode, alt, control, shift), default, 0, 0);
        }

        public static SampleEvent MouseMove(int x, int y)
        {
            return new SampleEvent(SampleEventType.MouseMoved, default, new SampleMouseEvent(SampleMouseButton.Unknown, x, y, 0), 0, 0);
        }

        public static SampleEvent MouseDown(SampleMouseButton button, int x, int y)
        {
            return new SampleEvent(SampleEventType.MouseButtonDown, default, new SampleMouseEvent(button, x, y, 0), 0, 0);
        }

        public static SampleEvent MouseUp(SampleMouseButton button, int x, int y)
        {
            return new SampleEvent(SampleEventType.MouseButtonUp, default, new SampleMouseEvent(button, x, y, 0), 0, 0);
        }

        public static SampleEvent MouseWheel(int x, int y, int delta)
        {
            return new SampleEvent(SampleEventType.MouseWheel, default, new SampleMouseEvent(SampleMouseButton.Unknown, x, y, delta), 0, 0);
        }

        public static SampleEvent Resized(int width, int height)
        {
            return new SampleEvent(SampleEventType.Resized, default, default, width, height);
        }

        public static SampleEvent FocusGained()
        {
            return new SampleEvent(SampleEventType.FocusGained, default, default, 0, 0);
        }

        public static SampleEvent FocusLost()
        {
            return new SampleEvent(SampleEventType.FocusLost, default, default, 0, 0);
        }
    }
}

