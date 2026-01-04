using AngleBridge.Managed.GLES;
using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    partial class AngleGl
    {
        public readonly struct CapsApi
        {
            private readonly AngleGl _gl;

            internal CapsApi(AngleGl gl)
            {
                _gl = gl;
            }

            public int GetInteger(GetIntegerParameter pname)
            {
                _gl.Guard();
                return Gles.GetInteger(U32((int)pname));
            }

            public bool TryGetInteger(GetIntegerParameter pname, out int value)
            {
                _gl.Guard();
                return Gles.TryGetInteger(U32((int)pname), out value);
            }

            public float GetFloat(GetFloatParameter pname)
            {
                _gl.Guard();
                return Gles.GetFloat(U32((int)pname));
            }

            public bool TryGetFloat(GetFloatParameter pname, out float value)
            {
                _gl.Guard();
                return Gles.TryGetFloat(U32((int)pname), out value);
            }

            public bool TryGetGpuDisjoint(out bool disjoint)
            {
                _gl.Guard();
                return Gles.TryGetBoolean(Gles.GL_GPU_DISJOINT_EXT, out disjoint);
            }
        }
    }
}
