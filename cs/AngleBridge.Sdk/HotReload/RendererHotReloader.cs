using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;

namespace AngleBridge.Sdk.HotReload
{
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
            if (string.IsNullOrWhiteSpace(pluginPath))
                throw new ArgumentException("Plugin path must be set.", nameof(pluginPath));

            _pluginPath = Path.GetFullPath(pluginPath);
            _debounce = debounce ?? TimeSpan.FromMilliseconds(250);
        }

        public IRenderer Current
        {
            get
            {
                if (_renderer == null)
                    throw new InvalidOperationException("Renderer not loaded yet. Call Start() first.");
                return _renderer;
            }
        }

        public Exception? LastError { get; private set; }

        public void Start()
        {
            if (_watcher != null) throw new InvalidOperationException("Already started.");

            ReloadCore(false);

            var dir = Path.GetDirectoryName(_pluginPath);
            if (string.IsNullOrWhiteSpace(dir))
                throw new InvalidOperationException(string.Format("Invalid plugin path: '{0}'.", _pluginPath));

            Directory.CreateDirectory(dir);

            var file = Path.GetFileName(_pluginPath);
            _watcher = new FileSystemWatcher(dir, file);
            _watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.LastWrite | NotifyFilters.CreationTime;
            _watcher.EnableRaisingEvents = true;
            _watcher.Changed += delegate { RequestReload(); };
            _watcher.Created += delegate { RequestReload(); };
            _watcher.Renamed += delegate { RequestReload(); };
            _watcher.Deleted += delegate { RequestReload(); };
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
                ReloadCore(true);
                LastError = null;
            }
            catch (Exception ex)
            {
                LastError = ex;
            }
        }

        public void Dispose()
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
                _watcher = null;
            }

            UnloadCurrent();
            GC.SuppressFinalize(this);
        }

        private void ReloadCore(bool isHotReload)
        {
            if (!File.Exists(_pluginPath))
                throw new FileNotFoundException(
                    string.Format("Plugin not found: '{0}'. Build it first.", _pluginPath),
                    _pluginPath);

            var tuple = LoadNew(_pluginPath, isHotReload);
            var newAlc = tuple.alc;
            var newRenderer = tuple.renderer;
            var newShadowDir = tuple.shadowDir;

            var oldRenderer = _renderer;
            var oldAlc = _alc;
            var oldAlcWeakRef = _alcWeakRef;
            var oldShadowDir = _shadowDir;

            _renderer = newRenderer;
            _alc = newAlc;
            _alcWeakRef = new WeakReference(newAlc, true);
            _shadowDir = newShadowDir;

            if (oldRenderer != null) oldRenderer.Dispose();
            if (oldAlc != null)
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

            if (renderer != null) renderer.Dispose();
            if (alc == null) return;

            alc.Unload();
            TryDeleteDirectory(shadowDir);
            TryForceUnload(alcWeakRef);
        }

        private static (PluginLoadContext alc, IRenderer renderer, string shadowDir) LoadNew(string pluginPath, bool isHotReload)
        {
            var shadowDir = CreateShadowDirectory(pluginPath);
            var shadowPluginPath = ShadowCopyPlugin(pluginPath, shadowDir);

            var alc = new PluginLoadContext(shadowPluginPath);
            try
            {
                var asm = alc.LoadFromAssemblyPath(shadowPluginPath);
                var rendererType = FindRendererType(asm);
                var rendererObj = Activator.CreateInstance(rendererType);
                if (rendererObj == null)
                    throw new InvalidOperationException(
                        string.Format("Failed to create renderer instance for '{0}'.", rendererType.FullName));

                var renderer = (IRenderer)rendererObj;
                var withCtx = rendererObj as IRendererWithHostContext;
                if (withCtx != null)
                    withCtx.SetHostContext(new RendererHostContext(pluginPath, shadowDir, isHotReload));
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
                types = ex.Types.Where(t => t != null).Cast<Type>().ToArray();
            }

            var contract = typeof(IRenderer);
            var t = types.FirstOrDefault(t =>
                t != null &&
                !t.IsAbstract &&
                contract.IsAssignableFrom(t) &&
                t.GetConstructor(Type.EmptyTypes) != null);

            if (t == null)
                throw new InvalidOperationException(
                    string.Format("No public parameterless type implementing {0} found in '{1}'.",
                        contract.FullName, asm.FullName));

            return t;
        }

        private static string CreateShadowDirectory(string pluginPath)
        {
            var pluginName = Path.GetFileNameWithoutExtension(pluginPath) ?? "plugin";
            var baseDir = Path.Combine(Path.GetTempPath(), "angle-dotnet", "hotreload", pluginName);
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
                    using (var srcStream = new FileStream(src, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                    using (var dstStream = new FileStream(dst, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        srcStream.CopyTo(dstStream);
                    }
                    return;
                }
                catch (Exception)
                {
                    if (attempt >= maxAttempts) throw;
                    Thread.Sleep(delayMs);
                }
            }
        }

        private static void TryForceUnload(WeakReference? alcWeakRef)
        {
            if (alcWeakRef == null) return;

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
                Directory.Delete(dir, true);
            }
            catch
            {
            }
        }

        private sealed class PluginLoadContext : AssemblyLoadContext
        {
            private readonly AssemblyDependencyResolver _resolver;

            public PluginLoadContext(string pluginPath)
                : base(true)
            {
                _resolver = new AssemblyDependencyResolver(pluginPath);
            }

            protected override Assembly? Load(AssemblyName assemblyName)
            {
                if (string.Equals(assemblyName.Name, "AngleBridge.Managed", StringComparison.Ordinal) ||
                    string.Equals(assemblyName.Name, "AngleBridge.Sdk", StringComparison.Ordinal))
                    return Assembly.Load(assemblyName);

                var path = _resolver.ResolveAssemblyToPath(assemblyName);
                return path == null ? null : LoadFromAssemblyPath(path);
            }

            protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
            {
                var path = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
                return path == null ? IntPtr.Zero : LoadUnmanagedDllFromPath(path);
            }
        }
    }
}
