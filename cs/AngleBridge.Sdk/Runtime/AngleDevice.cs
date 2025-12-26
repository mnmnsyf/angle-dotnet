using AngleBridge.Managed.GLES;
using AngleBridge.Managed.Runtime;
using AngleBridge.Sdk.Interop;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace AngleBridge.Sdk.Runtime
{
    public sealed class AngleDevice : IDisposable
    {
        private readonly int _ownerThreadId;
        private AngleContext? _context;
        private readonly bool _isWindow;
        private Gles.DebugProc? _debugCallback;

        private AngleDevice(AngleContext context, bool isWindow)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            _ownerThreadId = Environment.CurrentManagedThreadId;
            _context = context;
            _isWindow = isWindow;

            GlesLoader.Load();
            Gl = new AngleGl(this);
        }

        public static AngleDevice CreateWindow(IntPtr nativeWindow, AngleDeviceOptions options)
        {
            if (nativeWindow == IntPtr.Zero) throw new ArgumentException("Native window must be non-zero.", nameof(nativeWindow));
            if (options == null) throw new ArgumentNullException(nameof(options));

            var ctx = AngleContext.CreateWindow(
                nativeWindow,
                options.GlesMajor,
                (int)options.Backend,
                (int)options.DeviceType,
                options.SwapInterval);
            return new AngleDevice(ctx, isWindow: true);
        }

        public static AngleDevice CreateOffscreen(int width, int height, AngleDeviceOptions options)
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive.");
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive.");
            if (options == null) throw new ArgumentNullException(nameof(options));

            var ctx = AngleContext.CreateOffscreen(
                options.GlesMajor,
                width,
                height,
                (int)options.Backend,
                (int)options.DeviceType,
                options.SwapInterval);
            return new AngleDevice(ctx, isWindow: false);
        }

        public AngleGl Gl { get; }

        public int OwnerThreadId { get { return _ownerThreadId; } }

        public bool IsWindow { get { return _isWindow; } }

        internal bool IsDisposed { get { return _context == null; } }

        public void Present()
        {
            VerifyAccess();
            var ctx = _context;
            if (ctx == null) throw new ObjectDisposedException(nameof(AngleDevice));

            if (!_isWindow)
                throw new InvalidOperationException("Present() is only valid for window surfaces.");

            ctx.Swap();
        }

        public void ReadPixelsRgba(int x, int y, int width, int height, byte[] dst, int dstOffsetBytes)
        {
            VerifyAccess();
            ThrowIfDisposed();

            if (dst == null) throw new ArgumentNullException(nameof(dst));
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive.");
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive.");
            if (dstOffsetBytes < 0) throw new ArgumentOutOfRangeException(nameof(dstOffsetBytes), "Offset must be non-negative.");

            var required = checked(width * height * 4);
            if (dstOffsetBytes > dst.Length - required)
                throw new ArgumentException("Destination buffer too small for requested readback.", nameof(dst));

            if (dstOffsetBytes == 0)
            {
                Gles.ReadPixels(x, y, width, height, dst);
                return;
            }

            var tmp = new byte[required];
            Gles.ReadPixels(x, y, width, height, tmp);
            Buffer.BlockCopy(tmp, 0, dst, dstOffsetBytes, required);
        }

        public void NativeNoOp()
        {
            VerifyAccess();
            ThrowIfDisposed();
            global::AngleBridgeNative.AB_NoOp();
        }

        public void NativeNoOpN(int n)
        {
            VerifyAccess();
            ThrowIfDisposed();
            if (n <= 0) return;
            global::AngleBridgeNative.AB_NoOpN(n);
        }

        public void NativeGlCallBatch(int n)
        {
            VerifyAccess();
            ThrowIfDisposed();
            if (n <= 0) return;
            global::AngleBridgeNative.AB_GlCallBatch(n);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            var ctx = Interlocked.Exchange(ref _context, null);
            if (ctx != null) ctx.Dispose();
        }

        internal void VerifyAccess()
        {
            if (Environment.CurrentManagedThreadId != _ownerThreadId)
                throw new InvalidOperationException("ANGLE/EGL context is thread-affine. Use the creating thread, or add an explicit MakeCurrent/thread-dispatch layer.");
        }

        internal void ThrowIfDisposed()
        {
            if (_context == null) throw new ObjectDisposedException(nameof(AngleDevice));
        }

        public bool TryEnableDebugOutput(Action<string> logger, bool synchronous = true)
        {
            VerifyAccess();
            ThrowIfDisposed();
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (!Gles.IsDebugCallbackAvailable)
                return false;

            _debugCallback = (source, type, id, severity, length, message, userParam) =>
            {
                string text;
                if (message == IntPtr.Zero)
                {
                    text = string.Empty;
                }
                else
                {
                    text = length > 0
                        ? Marshal.PtrToStringUTF8(message, length) ?? string.Empty
                        : Marshal.PtrToStringUTF8(message) ?? string.Empty;
                }

                logger(string.Format(
                    "GL DEBUG: source=0x{0:X} type=0x{1:X} id={2} severity=0x{3:X} msg={4}",
                    source, type, id, severity, text));
            };

            Gles.DebugMessageCallback(_debugCallback, IntPtr.Zero);
            Gles.Enable(Gles.GL_DEBUG_OUTPUT);
            if (synchronous)
                Gles.Enable(Gles.GL_DEBUG_OUTPUT_SYNCHRONOUS);

            return true;
        }
    }
}
