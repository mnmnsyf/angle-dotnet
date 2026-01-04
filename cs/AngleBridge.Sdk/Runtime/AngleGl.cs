using AngleBridge.Managed.GLES;
using AngleBridge.Sdk.Interop;
using System;

namespace AngleBridge.Sdk.Runtime
{
    public sealed partial class AngleGl
    {
        private readonly AngleDevice _device;

        internal AngleGl(AngleDevice device)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));
            
            State = new StateApi(this);
            Shaders = new ShadersApi(this);
            Programs = new ProgramsApi(this);
            Buffers = new BuffersApi(this);
            VertexArrays = new VertexArraysApi(this);
            Draw = new DrawApi(this);
            Textures = new TexturesApi(this);
            Framebuffers = new FramebuffersApi(this);
            Renderbuffers = new RenderbuffersApi(this);
            Samplers = new SamplersApi(this);
            Queries = new QueriesApi(this);
            Sync = new SyncApi(this);
            Caps = new CapsApi(this);
            TransformFeedback = new TransformFeedbackApi(this);
        }

        internal AngleDevice Device { get { return _device; } }

        // API Modules
        public StateApi State { get; }
        public ShadersApi Shaders { get; }
        public ProgramsApi Programs { get; }
        public BuffersApi Buffers { get; }
        public VertexArraysApi VertexArrays { get; }
        public DrawApi Draw { get; }
        public TexturesApi Textures { get; }
        public FramebuffersApi Framebuffers { get; }
        public RenderbuffersApi Renderbuffers { get; }
        public SamplersApi Samplers { get; }
        public QueriesApi Queries { get; }
        public SyncApi Sync { get; }
        public CapsApi Caps { get; }
        public TransformFeedbackApi TransformFeedback { get; }

        private void Guard()
        {
            _device.VerifyAccess();
            _device.ThrowIfDisposed();
        }

        private static uint U32(int v) { return unchecked((uint)v); }
        private static int I32(uint v) { return unchecked((int)v); }

        private void ValidateShader(AngleShader shader)
        {
            if (shader == null) throw new ArgumentNullException(nameof(shader));
            if (!ReferenceEquals(shader.Device, _device))
                throw new InvalidOperationException("Shader does not belong to this device.");
            shader.ThrowIfDisposed();
        }

        private void ValidateProgram(AngleProgram program)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            if (!ReferenceEquals(program.Device, _device))
                throw new InvalidOperationException("Program does not belong to this device.");
            program.ThrowIfDisposed();
        }

        private void ValidateBuffer(AngleBuffer buffer)
        {
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (!ReferenceEquals(buffer.Device, _device))
                throw new InvalidOperationException("Buffer does not belong to this device.");
            buffer.ThrowIfDisposed();
        }

        private void ValidateVertexArray(AngleVertexArray vao)
        {
            if (vao == null) throw new ArgumentNullException(nameof(vao));
            if (!ReferenceEquals(vao.Device, _device))
                throw new InvalidOperationException("Vertex array does not belong to this device.");
            vao.ThrowIfDisposed();
        }

        private void ValidateTexture(AngleTexture texture)
        {
            if (texture == null) throw new ArgumentNullException(nameof(texture));
            if (!ReferenceEquals(texture.Device, _device))
                throw new InvalidOperationException("Texture does not belong to this device.");
            texture.ThrowIfDisposed();
        }

        private void ValidateFramebuffer(AngleFramebuffer framebuffer)
        {
            if (framebuffer == null) throw new ArgumentNullException(nameof(framebuffer));
            if (!ReferenceEquals(framebuffer.Device, _device))
                throw new InvalidOperationException("Framebuffer does not belong to this device.");
            framebuffer.ThrowIfDisposed();
        }

        private void ValidateRenderbuffer(AngleRenderbuffer renderbuffer)
        {
            if (renderbuffer == null) throw new ArgumentNullException(nameof(renderbuffer));
            if (!ReferenceEquals(renderbuffer.Device, _device))
                throw new InvalidOperationException("Renderbuffer does not belong to this device.");
            renderbuffer.ThrowIfDisposed();
        }

        private void ValidateSampler(AngleSampler sampler)
        {
            if (sampler == null) throw new ArgumentNullException(nameof(sampler));
            if (!ReferenceEquals(sampler.Device, _device))
                throw new InvalidOperationException("Sampler does not belong to this device.");
            sampler.ThrowIfDisposed();
        }

        private void ValidateQuery(AngleQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (!ReferenceEquals(query.Device, _device))
                throw new InvalidOperationException("Query does not belong to this device.");
            query.ThrowIfDisposed();
        }

        private void ValidateSync(AngleSync sync)
        {
            if (sync == null) throw new ArgumentNullException(nameof(sync));
            if (!ReferenceEquals(sync.Device, _device))
                throw new InvalidOperationException("Sync does not belong to this device.");
            sync.ThrowIfDisposed();
        }

        private void ValidateTransformFeedback(AngleTransformFeedback tf)
        {
            if (tf == null) throw new ArgumentNullException(nameof(tf));
            if (!ReferenceEquals(tf.Device, _device))
                throw new InvalidOperationException("Transform feedback does not belong to this device.");
            tf.ThrowIfDisposed();
        }
    }
}
