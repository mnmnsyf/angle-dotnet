using AngleBridge.Managed.HotReload;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;

namespace Demo3_Triangle.WinForms.HotReload;

public sealed class RendererHotReloader : IDisposable
{
    private readonly string _pluginPath;
    private readonly TimeSpan _debounce;

    private FileSystemWatcher? _watcher;
    private int _reloadRequested;
    private DateTime _lastChangeUtc;

    private PluginLoadContext? _alc;
    private WeakReference? _alcWeakRef;
    private string? _shadowDir;

    private IRenderer? _renderer;

    public RendererHotReloader(string pluginPath, TimeSpan? debounce = null)
    {
        if (string.IsNullOrWhiteSpace(pluginPath)) throw new ArgumentException("Plugin path must be set.", nameof(pluginPath));
        _pluginPath = Path.GetFullPath(pluginPath);
        _debounce = debounce ?? TimeSpan.FromMilliseconds(250);
    }

    public IRenderer Current => _renderer ?? throw new InvalidOperationException("Renderer not loaded yet. Call Start() first.");

    public Exception? LastError { get; private set; }

    public void Start()
    {
        if (_watcher is not null) throw new InvalidOperationException("Already started.");

        ReloadCore(isHotReload: false);

        var dir = Path.GetDirectoryName(_pluginPath);
        if (string.IsNullOrWhiteSpace(dir)) throw new InvalidOperationException($"Invalid plugin path: '{_pluginPath}'.");
        Directory.CreateDirectory(dir);

        var file = Path.GetFileName(_pluginPath);
        _watcher = new FileSystemWatcher(dir, file)
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.CreationTime,
            EnableRaisingEvents = true,
        };
        _watcher.Changed += (_, _) => RequestReload();
        _watcher.Created += (_, _) => RequestReload();
        _watcher.Renamed += (_, _) => RequestReload();
        _watcher.Deleted += (_, _) => RequestReload();
    }

    public void RequestReload()
    {
        _lastChangeUtc = DateTime.UtcNow;
        Volatile.Write(ref _reloadRequested, 1);
    }

    public void Tick()
    {
        if (Volatile.Read(ref _reloadRequested) == 0) return;
        if (DateTime.UtcNow - _lastChangeUtc < _debounce) return;
        Volatile.Write(ref _reloadRequested, 0);

        try
        {
            ReloadCore(isHotReload: true);
            LastError = null;
        }
        catch (Exception ex)
        {
            LastError = ex;
        }
    }

    public void Dispose()
    {
        _watcher?.Dispose();
        _watcher = null;

        UnloadCurrent();
        GC.SuppressFinalize(this);
    }

    private void ReloadCore(bool isHotReload)
    {
        if (!File.Exists(_pluginPath))
            throw new FileNotFoundException($"Plugin not found: '{_pluginPath}'. Build it first.", _pluginPath);

        var (newAlc, newRenderer, newShadowDir) = LoadNew(_pluginPath);

        var oldRenderer = _renderer;
        var oldAlc = _alc;
        var oldAlcWeakRef = _alcWeakRef;
        var oldShadowDir = _shadowDir;

        _renderer = newRenderer;
        _alc = newAlc;
        _alcWeakRef = new WeakReference(newAlc, trackResurrection: true);
        _shadowDir = newShadowDir;

        oldRenderer?.Dispose();
        if (oldAlc is not null)
        {
            oldAlc.Unload();
            TryDeleteDirectory(oldShadowDir);
            TryForceUnload(oldAlcWeakRef);
        }
    }

    private void UnloadCurrent()
    {
        var renderer = _renderer;
        var alc = _alc;
        var alcWeakRef = _alcWeakRef;
        var shadowDir = _shadowDir;

        _renderer = null;
        _alc = null;
        _alcWeakRef = null;
        _shadowDir = null;

        renderer?.Dispose();
        if (alc is null) return;

        alc.Unload();
        TryDeleteDirectory(shadowDir);
        TryForceUnload(alcWeakRef);
    }

    private static (PluginLoadContext alc, IRenderer renderer, string shadowDir) LoadNew(string pluginPath)
    {
        var shadowDir = CreateShadowDirectory();
        var shadowPluginPath = ShadowCopyPlugin(pluginPath, shadowDir);

        var alc = new PluginLoadContext(shadowPluginPath);
        try
        {
            var asm = alc.LoadFromAssemblyPath(shadowPluginPath);
            var rendererType = FindRendererType(asm);
            var renderer = (IRenderer)Activator.CreateInstance(rendererType)!;
            return (alc, renderer, shadowDir);
        }
        catch
        {
            alc.Unload();
            TryDeleteDirectory(shadowDir);
            throw;
        }
    }

    private static Type FindRendererType(Assembly asm)
    {
        Type[] types;
        try
        {
            types = asm.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            types = ex.Types.Where(t => t is not null).Cast<Type>().ToArray();
        }

        var contract = typeof(IRenderer);
        var t = types.FirstOrDefault(t =>
            t is not null &&
            !t.IsAbstract &&
            contract.IsAssignableFrom(t) &&
            t.GetConstructor(Type.EmptyTypes) is not null);

        if (t is null)
            throw new InvalidOperationException($"No public parameterless type implementing {contract.FullName} found in '{asm.FullName}'.");

        return t;
    }

    private static string CreateShadowDirectory()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "angle-dotnet", "demo3", "shadow");
        var dir = Path.Combine(baseDir, Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }

    private static string ShadowCopyPlugin(string pluginPath, string shadowDir)
    {
        var dllName = Path.GetFileName(pluginPath);
        var dstDll = Path.Combine(shadowDir, dllName);
        CopyFileWithRetry(pluginPath, dstDll);

        var pdbSrc = Path.ChangeExtension(pluginPath, ".pdb");
        if (File.Exists(pdbSrc))
        {
            var pdbName = Path.GetFileName(pdbSrc);
            CopyFileWithRetry(pdbSrc, Path.Combine(shadowDir, pdbName));
        }

        return dstDll;
    }

    private static void CopyFileWithRetry(string src, string dst)
    {
        const int maxAttempts = 50;
        const int delayMs = 20;

        for (var attempt = 1; ; attempt++)
        {
            try
            {
                using var srcStream = new FileStream(src, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                using var dstStream = new FileStream(dst, FileMode.Create, FileAccess.Write, FileShare.None);
                srcStream.CopyTo(dstStream);
                return;
            }
            catch (Exception) when (attempt < maxAttempts)
            {
                Thread.Sleep(delayMs);
            }
        }
    }

    private static void TryForceUnload(WeakReference? alcWeakRef)
    {
        if (alcWeakRef is null) return;

        for (var i = 0; i < 8 && alcWeakRef.IsAlive; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Thread.Sleep(10);
        }
    }

    private static void TryDeleteDirectory(string? dir)
    {
        if (string.IsNullOrWhiteSpace(dir)) return;
        try
        {
            Directory.Delete(dir, recursive: true);
        }
        catch
        {
        }
    }

    private sealed class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath)
            : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            if (string.Equals(assemblyName.Name, "AngleBridge.Managed", StringComparison.Ordinal))
                return Assembly.Load(assemblyName);

            var path = _resolver.ResolveAssemblyToPath(assemblyName);
            return path is null ? null : LoadFromAssemblyPath(path);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var path = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            return path is null ? IntPtr.Zero : LoadUnmanagedDllFromPath(path);
        }
    }
}
