using AngleBridge.Sdk.Runtime;

namespace AngleBridge.Sdk.HotReload
{
    public interface IRendererWithPresent
    {
        void Present(AngleDevice device, int width, int height);
    }
}
