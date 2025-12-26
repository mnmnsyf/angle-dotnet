using AngleBridge.Sdk.Runtime;
using System;

namespace AngleBridge.Utilities.WinForms
{
    public sealed class WinFormsHostOptions
    {
        public string Title { get; set; } = "ANGLE Host";
        public int Width { get; set; } = 640;
        public int Height { get; set; } = 480;
        public int Frames { get; set; } = 0;
        public bool Visible { get; set; }
        public int SwapInterval { get; set; } = 0;
        public int GlesMajor { get; set; } = 3;
        public AngleBackend Backend { get; set; } = AngleBackend.Default;
        public AngleDeviceType DeviceType { get; set; } = AngleDeviceType.Default;
        public bool EnableDebug { get; set; }
        public bool DebugSynchronous { get; set; } = true;
        public string? PluginPath { get; set; }

        public WinFormsHostOptions Clone()
        {
            return new WinFormsHostOptions
            {
                Title = Title,
                Width = Width,
                Height = Height,
                Frames = Frames,
                Visible = Visible,
                SwapInterval = SwapInterval,
                GlesMajor = GlesMajor,
                Backend = Backend,
                DeviceType = DeviceType,
                EnableDebug = EnableDebug,
                DebugSynchronous = DebugSynchronous,
                PluginPath = PluginPath,
            };
        }
    }
}
