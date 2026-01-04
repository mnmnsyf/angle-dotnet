using AngleBridge.Managed.GLES;
using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    partial class AngleGl
    {
        public readonly struct SamplersApi
        {
            private readonly AngleGl _gl;

            internal SamplersApi(AngleGl gl)
            {
                _gl = gl;
            }

            public GlSamplerHandle GenSampler()
            {
                _gl.Guard();
                return new GlSamplerHandle(I32(Gles.GenSampler()));
            }

            public AngleSampler CreateSamplerObject()
            {
                _gl.Guard();
                var handle = new GlSamplerHandle(I32(Gles.GenSampler()));
                return new AngleSampler(_gl, handle);
            }

            public void DeleteSampler(GlSamplerHandle sampler)
            {
                _gl.Guard();
                if (sampler.IsNull) return;
                Gles.DeleteSampler(U32(sampler.Value));
            }

            public void DeleteSampler(AngleSampler sampler)
            {
                _gl.Guard();
                _gl.ValidateSampler(sampler);
                Gles.DeleteSampler(U32(sampler.Handle.Value));
                sampler.Dispose();
            }

            public void BindSampler(int unit, GlSamplerHandle sampler)
            {
                _gl.Guard();
                if (unit < 0) throw new ArgumentOutOfRangeException(nameof(unit));
                Gles.BindSampler(U32(unit), U32(sampler.Value));
            }

            public void BindSampler(int unit, AngleSampler sampler)
            {
                _gl.Guard();
                _gl.ValidateSampler(sampler);
                if (unit < 0) throw new ArgumentOutOfRangeException(nameof(unit));
                Gles.BindSampler(U32(unit), U32(sampler.Handle.Value));
            }

            public void SamplerParameteri(GlSamplerHandle sampler, TextureParameterName pname, int param)
            {
                _gl.Guard();
                Gles.SamplerParameteri(U32(sampler.Value), U32((int)pname), param);
            }

            public void SamplerParameteri(AngleSampler sampler, TextureParameterName pname, int param)
            {
                _gl.Guard();
                _gl.ValidateSampler(sampler);
                Gles.SamplerParameteri(U32(sampler.Handle.Value), U32((int)pname), param);
            }

            public void SamplerParameterf(GlSamplerHandle sampler, TextureParameterName pname, float param)
            {
                _gl.Guard();
                Gles.SamplerParameterf(U32(sampler.Value), U32((int)pname), param);
            }

            public void SamplerParameterf(AngleSampler sampler, TextureParameterName pname, float param)
            {
                _gl.Guard();
                _gl.ValidateSampler(sampler);
                Gles.SamplerParameterf(U32(sampler.Handle.Value), U32((int)pname), param);
            }
        }
    }
}
