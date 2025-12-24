using AngleBridge.Sdk.Runtime;
using Demo3_Triangle.WinForms.HotReload;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Demo3_Triangle.WinForms
{
    public sealed class MainForm : Form
    {
        private sealed class RenderPanel : Panel
        {
            public RenderPanel()
            {
                SetStyle(ControlStyles.Opaque, true);
                BackColor = Color.Black;
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
            }
        }

        private readonly RenderPanel _panel;
        private readonly System.Windows.Forms.Timer _timer;
        private readonly RendererHotReloader _reloader;

        private AngleDevice? _device;
        private bool _rendering;

        public MainForm()
        {
            Text = "Demo3 - Triangle (ANGLE + WinForms)";
            ClientSize = new Size(960, 540);

            _panel = new RenderPanel { Dock = DockStyle.Fill };
            Controls.Add(_panel);

            _timer = new System.Windows.Forms.Timer { Interval = 16 };
            _timer.Tick += OnTimerTick;

            var pluginPath = ResolvePluginPath();
            _reloader = new RendererHotReloader(pluginPath);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            try
            {
                _device = AngleDevice.CreateWindow(_panel.Handle, new AngleDeviceOptions { GlesMajor = 3 });
                _reloader.Start();
                _timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Init failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _timer.Stop();
            _reloader.Dispose();
            if (_device != null) _device.Dispose();
            base.OnFormClosed(e);
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            RenderFrame();
        }

        private void RenderFrame()
        {
            if (_device == null) return;
            if (_rendering) return;

            var w = _panel.ClientSize.Width;
            var h = _panel.ClientSize.Height;
            if (w <= 0 || h <= 0) return;

            try
            {
                _rendering = true;
                _reloader.Tick();
                _reloader.Current.Draw(_device.Gl, w, h);
                _device.Present();
            }
            catch (Exception ex)
            {
                _timer.Stop();
                MessageBox.Show(this, ex.ToString(), "Render failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _rendering = false;
            }
        }

        private static string ResolvePluginPath()
        {
            var baseDir = AppContext.BaseDirectory;
#if DEBUG
            const string configuration = "Debug";
#else
            const string configuration = "Release";
#endif
            var current = new DirectoryInfo(baseDir);
            for (var i = 0; i < 6 && current != null; i++)
            {
                var candidate = Path.Combine(
                    current.FullName,
                    "demos",
                    "Demo3_Triangle.Plugin",
                    "bin",
                    configuration,
                    "net9.0",
                    "Demo3_Triangle.Plugin.dll");
                if (File.Exists(candidate)) return candidate;
                current = current.Parent;
            }

            return Path.Combine(baseDir, "plugins", "Demo3_Triangle.Plugin.dll");
        }
    }

    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}
