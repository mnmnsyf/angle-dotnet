namespace AngleBridge.Managed.HotReload;

public interface IRenderer : IDisposable
{
    void Draw(int width, int height);
}

