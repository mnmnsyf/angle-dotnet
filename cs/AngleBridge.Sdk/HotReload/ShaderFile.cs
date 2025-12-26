using System;
using System.IO;
using System.Text;
using System.Threading;

namespace AngleBridge.Sdk.HotReload
{
    public sealed class ShaderFile
    {
        private readonly string _path;
        private readonly string[] _searchRoots;
        private string? _resolvedPath;
        private DateTime _lastWriteUtc;

        public ShaderFile(string path, params string[] searchRoots)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Shader path must be set.", nameof(path));

            _path = path;
            _searchRoots = searchRoots != null && searchRoots.Length > 0
                ? searchRoots
                : GetDefaultSearchRoots();
        }

        public Exception? LastError { get; private set; }

        public string Load()
        {
            var path = ResolvePath();
            var source = ReadAllTextWithRetry(path);
            _lastWriteUtc = File.GetLastWriteTimeUtc(path);
            UpdateLastError(null);
            return source;
        }

        public bool TryReload(out string source)
        {
            var path = ResolvePath();
            source = string.Empty;
            var lastWrite = File.GetLastWriteTimeUtc(path);
            if (lastWrite <= _lastWriteUtc)
            {
                return false;
            }

            const int debounceMs = 100;
            if ((DateTime.UtcNow - lastWrite).TotalMilliseconds < debounceMs)
                return false;

            var lastLength = TryGetFileLength(path);

            for (var stableAttempt = 0; stableAttempt < 5; stableAttempt++)
            {
                if (!TryReadAllTextWithRetry(path, 10, 5, out source, out var readError))
                {
                    UpdateLastError(readError);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(source))
                {
                    UpdateLastError(new IOException("Shader file read empty (maybe mid-save): " + path));
                    return false;
                }

                var afterReadWrite = File.GetLastWriteTimeUtc(path);
                var afterReadLength = TryGetFileLength(path);
                if (afterReadWrite == lastWrite)
                {
                    if (afterReadLength.HasValue && lastLength.HasValue && afterReadLength.Value != lastLength.Value)
                    {
                        lastWrite = afterReadWrite;
                        lastLength = afterReadLength;
                        Thread.Sleep(5);
                        continue;
                    }

                    _lastWriteUtc = lastWrite;
                    UpdateLastError(null);
                    return true;
                }

                lastWrite = afterReadWrite;
                lastLength = afterReadLength;
                Thread.Sleep(5);
            }

            UpdateLastError(new IOException("Shader file is changing too frequently; will retry: " + path));
            return false;
        }

        public string ResolvedPath { get { return ResolvePath(); } }

        private string ResolvePath()
        {
            if (_resolvedPath != null) return _resolvedPath;

            if (Path.IsPathRooted(_path))
            {
                if (!File.Exists(_path))
                    throw new FileNotFoundException("Shader file not found: " + _path, _path);

                _resolvedPath = _path;
                return _resolvedPath;
            }

            foreach (var root in _searchRoots)
            {
                if (string.IsNullOrWhiteSpace(root)) continue;
                var candidate = Path.Combine(root, _path);
                if (File.Exists(candidate))
                {
                    _resolvedPath = candidate;
                    return candidate;
                }
            }

            throw new FileNotFoundException("Shader file not found: " + _path, _path);
        }

        private static string[] GetDefaultSearchRoots()
        {
            return new[]
            {
                AppContext.BaseDirectory,
                Directory.GetCurrentDirectory(),
            };
        }

        private static string ReadAllTextWithRetry(string path)
        {
            if (!TryReadAllTextWithRetry(path, 200, 5, out var text, out var error))
                throw error ?? new IOException("Failed to read shader file: " + path);
            return text;
        }

        private static bool TryReadAllTextWithRetry(string path, int maxAttempts, int delayMs, out string text, out Exception? error)
        {
            if (maxAttempts <= 0) throw new ArgumentOutOfRangeException(nameof(maxAttempts));
            if (delayMs < 0) throw new ArgumentOutOfRangeException(nameof(delayMs));

            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    using (var stream = new FileStream(
                        path,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.ReadWrite | FileShare.Delete))
                    using (var reader = new StreamReader(stream, Encoding.UTF8, true))
                    {
                        text = reader.ReadToEnd();
                        error = null;
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    if (!(ex is IOException) && !(ex is UnauthorizedAccessException))
                        throw;

                    if (attempt >= maxAttempts)
                    {
                        text = string.Empty;
                        error = ex;
                        return false;
                    }
                    if (delayMs > 0)
                        Thread.Sleep(delayMs);
                }
            }

            text = string.Empty;
            error = new IOException("Failed to read shader file: " + path);
            return false;
        }

        private void UpdateLastError(Exception? error)
        {
            if (error == null)
            {
                LastError = null;
                return;
            }

            var current = LastError;
            if (current != null &&
                current.GetType() == error.GetType() &&
                string.Equals(current.Message, error.Message, StringComparison.Ordinal))
            {
                return;
            }

            LastError = error;
        }

        private static long? TryGetFileLength(string path)
        {
            try
            {
                var info = new FileInfo(path);
                return info.Exists ? (long?)info.Length : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
