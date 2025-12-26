using System;
using System.IO;

namespace AngleBridge.Sdk.HotReload
{
    public sealed class RendererHostContext
    {
        public RendererHostContext(string pluginPath, string shadowDirectory, bool isHotReload)
        {
            if (string.IsNullOrWhiteSpace(pluginPath))
                throw new ArgumentException("Plugin path must be set.", nameof(pluginPath));
            if (string.IsNullOrWhiteSpace(shadowDirectory))
                throw new ArgumentException("Shadow directory must be set.", nameof(shadowDirectory));

            PluginPath = pluginPath;
            ShadowDirectory = shadowDirectory;
            IsHotReload = isHotReload;
            PluginDirectory = Path.GetDirectoryName(pluginPath) ?? string.Empty;
        }

        public string PluginPath { get; }
        public string PluginDirectory { get; }
        public string ShadowDirectory { get; }
        public bool IsHotReload { get; }
    }

    public interface IRendererWithHostContext
    {
        void SetHostContext(RendererHostContext context);
    }
}

