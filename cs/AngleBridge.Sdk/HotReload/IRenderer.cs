using AngleBridge.Sdk.Runtime;
using System;

namespace AngleBridge.Sdk.HotReload
{
    public interface IRenderer : IDisposable
    {
        void Draw(AngleGl gl, int width, int height);
    }
}

