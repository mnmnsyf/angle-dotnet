using AngleBridge.Managed.GLES;
using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    partial class AngleGl
    {
        public readonly struct SyncApi
        {
            private readonly AngleGl _gl;

            internal SyncApi(AngleGl gl)
            {
                _gl = gl;
            }

            public AngleSync FenceSync(SyncCondition condition = SyncCondition.SyncGpuCommandsComplete, SyncFlags flags = SyncFlags.None)
            {
                _gl.Guard();
                var sync = Gles.FenceSync(U32((int)condition), U32((int)flags));
                if (sync == IntPtr.Zero)
                    throw new InvalidOperationException("glFenceSync returned NULL.");
                return new AngleSync(_gl, new GlSyncHandle(sync));
            }

            public void DeleteSync(AngleSync sync)
            {
                _gl.Guard();
                _gl.ValidateSync(sync);
                Gles.DeleteSync(sync.Handle.Value);
                sync.Dispose();
            }

            [CLSCompliant(false)]
            public void DeleteSync(GlSyncHandle sync)
            {
                _gl.Guard();
                if (sync.IsNull) return;
                Gles.DeleteSync(sync.Value);
            }

            [CLSCompliant(false)]
            public ClientWaitSyncResult ClientWaitSync(AngleSync sync, SyncFlags flags, ulong timeoutNanoseconds)
            {
                _gl.Guard();
                _gl.ValidateSync(sync);
                var result = Gles.ClientWaitSync(sync.Handle.Value, U32((int)flags), timeoutNanoseconds);
                return (ClientWaitSyncResult)result;
            }

            [CLSCompliant(false)]
            public void WaitSync(AngleSync sync, SyncFlags flags, ulong timeoutNanoseconds)
            {
                _gl.Guard();
                _gl.ValidateSync(sync);
                Gles.WaitSync(sync.Handle.Value, U32((int)flags), timeoutNanoseconds);
            }
        }
    }
}
