using AngleBridge.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AngleBridge.Utilities.WinForms
{
    public sealed class WinFormsSampleWindow : ISampleWindow
    {
        private readonly Form _form;
        private readonly Queue<SampleEvent> _events = new Queue<SampleEvent>();
        private bool _visible;

        public WinFormsSampleWindow(string title, int width, int height, bool visible)
        {
            _form = new Form
            {
                Text = title,
                ClientSize = new Size(width, height),
                StartPosition = FormStartPosition.Manual,
                Location = visible ? new Point(100, 100) : new Point(-32000, -32000),
                ShowInTaskbar = visible,
                FormBorderStyle = visible ? FormBorderStyle.Sizable : FormBorderStyle.FixedToolWindow,
                Opacity = visible ? 1.0 : 0.0,
                KeyPreview = true,
            };

            _visible = visible;

            _form.FormClosed += (_, __) => _events.Enqueue(SampleEvent.Closed());
            _form.KeyDown += (_, e) => _events.Enqueue(SampleEvent.KeyDown((int)e.KeyCode, e.Alt, e.Control, e.Shift));
            _form.KeyUp += (_, e) => _events.Enqueue(SampleEvent.KeyUp((int)e.KeyCode, e.Alt, e.Control, e.Shift));

            _form.MouseMove += (_, e) => _events.Enqueue(SampleEvent.MouseMove(e.X, e.Y));
            _form.MouseDown += (_, e) => _events.Enqueue(SampleEvent.MouseDown(MapButton(e.Button), e.X, e.Y));
            _form.MouseUp += (_, e) => _events.Enqueue(SampleEvent.MouseUp(MapButton(e.Button), e.X, e.Y));
            _form.MouseWheel += (_, e) => _events.Enqueue(SampleEvent.MouseWheel(e.X, e.Y, e.Delta));

            _form.ClientSizeChanged += (_, __) => _events.Enqueue(SampleEvent.Resized(_form.ClientSize.Width, _form.ClientSize.Height));
            _form.GotFocus += (_, __) => _events.Enqueue(SampleEvent.FocusGained());
            _form.LostFocus += (_, __) => _events.Enqueue(SampleEvent.FocusLost());

            _form.Show();
        }

        public IntPtr NativeHandle { get { return _form.Handle; } }
        public int Width { get { return _form.ClientSize.Width; } }
        public int Height { get { return _form.ClientSize.Height; } }
        public int X { get { return _form.Location.X; } }
        public int Y { get { return _form.Location.Y; } }
        public bool Visible { get { return _visible; } }

        public void SetTitle(string title)
        {
            _form.Text = title;
        }

        public void SetPosition(int x, int y)
        {
            _form.Location = new Point(x, y);
        }

        public void SetSize(int width, int height)
        {
            _form.ClientSize = new Size(width, height);
        }

        public void SetVisible(bool visible)
        {
            _visible = visible;
            if (visible)
            {
                _form.Opacity = 1.0;
                _form.ShowInTaskbar = true;
                _form.FormBorderStyle = FormBorderStyle.Sizable;
                if (!_form.Visible)
                    _form.Show();
            }
            else
            {
                _form.Opacity = 0.0;
                _form.ShowInTaskbar = false;
                _form.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                if (!_form.Visible)
                    _form.Show();
            }
        }

        public void PumpEvents()
        {
            Application.DoEvents();
        }

        public bool TryDequeueEvent(out SampleEvent ev)
        {
            if (_events.Count == 0)
            {
                ev = default;
                return false;
            }

            ev = _events.Dequeue();
            return true;
        }

        public void Dispose()
        {
            if (_form.IsDisposed)
                return;

            try
            {
                _form.Close();
                _form.Dispose();
            }
            catch
            {
            }
        }

        private static SampleMouseButton MapButton(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    return SampleMouseButton.Left;
                case MouseButtons.Right:
                    return SampleMouseButton.Right;
                case MouseButtons.Middle:
                    return SampleMouseButton.Middle;
                case MouseButtons.XButton1:
                    return SampleMouseButton.XButton1;
                case MouseButtons.XButton2:
                    return SampleMouseButton.XButton2;
                default:
                    return SampleMouseButton.Unknown;
            }
        }
    }
}

