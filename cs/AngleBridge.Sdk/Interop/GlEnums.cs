namespace AngleBridge.Sdk.Interop
{
    [System.Flags]
    public enum ClearBufferMask : int
    {
        ColorBufferBit = 0x00004000,
        DepthBufferBit = 0x00000100,
        StencilBufferBit = 0x00000400,
    }

    public enum PrimitiveType : int
    {
        Points = 0x0000,
        TriangleFan = 0x0006,
        Triangles = 0x0004,
    }

    public enum BufferTarget : int
    {
        ArrayBuffer = 0x8892,
        ElementArrayBuffer = 0x8893,
        TransformFeedbackBuffer = 0x8C8E,
        UniformBuffer = 0x8A11,
    }

    public enum BufferUsage : int
    {
        StaticDraw = 0x88E4,
    }

    public enum VertexAttribType : int
    {
        Float = 0x1406,
    }

    public enum EnableCap : int
    {
        Blend = 0x0BE2,
        CullFace = 0x0B44,
        DepthTest = 0x0B71,
        StencilTest = 0x0B90,
    }

    public enum BlendFactor : int
    {
        One = 1,
        SrcAlpha = 0x0302,
    }

    public enum CullFaceMode : int
    {
        Back = 0x0405,
    }

    public enum ShaderType : int
    {
        VertexShader = 0x8B31,
        FragmentShader = 0x8B30,
    }

    public enum TextureTarget : int
    {
        Texture2D = 0x0DE1,
        TextureCubeMap = 0x8513,
        TextureCubeMapPositiveX = 0x8515,
        TextureCubeMapNegativeX = 0x8516,
        TextureCubeMapPositiveY = 0x8517,
        TextureCubeMapNegativeY = 0x8518,
        TextureCubeMapPositiveZ = 0x8519,
        TextureCubeMapNegativeZ = 0x851A,
    }

    public enum TextureParameterName : int
    {
        TextureMinFilter = 0x2801,
        TextureMagFilter = 0x2800,
        TextureWrapS = 0x2802,
        TextureWrapT = 0x2803,
        TextureMaxAnisotropyExt = 0x84FE,
    }

    public enum TextureMinFilter : int
    {
        Nearest = 0x2600,
        Linear = 0x2601,
        NearestMipmapNearest = 0x2700,
        LinearMipmapLinear = 0x2703,
    }

    public enum TextureMagFilter : int
    {
        Nearest = 0x2600,
        Linear = 0x2601,
    }

    public enum TextureWrapMode : int
    {
        Repeat = 0x2901,
        ClampToEdge = 0x812F,
        MirroredRepeat = 0x8370,
    }

    public enum TextureUnit : int
    {
        Texture0 = 0x84C0,
        Texture1 = 0x84C1,
        Texture2 = 0x84C2,
    }

    public enum FramebufferTarget : int
    {
        Framebuffer = 0x8D40,
        ReadFramebuffer = 0x8CA8,
        DrawFramebuffer = 0x8CA9,
    }

    public enum FramebufferAttachment : int
    {
        ColorAttachment0 = 0x8CE0,
        DepthAttachment = 0x8D00,
        StencilAttachment = 0x8D20,
        DepthStencilAttachment = 0x821A,
    }

    public enum FramebufferStatus : int
    {
        FramebufferComplete = 0x8CD5,
        FramebufferUnsupported = 0x8CDD,
    }

    public enum RenderbufferTarget : int
    {
        Renderbuffer = 0x8D41,
    }

    public enum RenderbufferInternalFormat : int
    {
        DepthComponent16 = 0x81A5,
        DepthComponent24 = 0x81A6,
        Depth24Stencil8 = 0x88F0,
        StencilIndex8 = 0x8D48,
    }

    public enum PixelStoreParameter : int
    {
        UnpackAlignment = 0x0CF5,
    }

    public enum PixelFormat : int
    {
        Rgb = 0x1907,
        Rgba = 0x1908,
    }

    public enum PixelType : int
    {
        UnsignedByte = 0x1401,
    }

    public enum IndexElementType : int
    {
        UnsignedShort = 0x1403,
    }

    public enum TextureInternalFormat : int
    {
        Rgb = 0x1907,
        Rgba = 0x1908,
        DepthComponent = 0x1902,
        DepthStencil = 0x84F9,
    }

    public enum ShaderParameter : int
    {
        CompileStatus = 0x8B81,
        InfoLogLength = 0x8B84,
    }

    public enum ProgramParameter : int
    {
        LinkStatus = 0x8B82,
        InfoLogLength = 0x8B84,
    }

    public enum QueryTarget : int
    {
        TimeElapsed = 0x88BF,
        AnySamplesPassed = 0x8C2F,
        TransformFeedbackPrimitivesWritten = 0x8C88,
    }

    public enum QueryParameter : int
    {
        QueryResult = 0x8866,
        QueryResultAvailable = 0x8867,
    }

    public enum StencilFunction : int
    {
        Never = 0x0200,
        Less = 0x0201,
        Equal = 0x0202,
        Lequal = 0x0203,
        Greater = 0x0204,
        Notequal = 0x0205,
        Gequal = 0x0206,
        Always = 0x0207,
    }

    public enum StencilOp : int
    {
        Keep = 0x1E00,
        Replace = 0x1E01,
        Incr = 0x1E02,
        Decr = 0x1E03,
        Invert = 0x150A,
        IncrWrap = 0x8507,
        DecrWrap = 0x8508,
    }

    public enum GetIntegerParameter : int
    {
        StencilBits = 0x0D57,
    }

    public enum GetFloatParameter : int
    {
        MaxTextureMaxAnisotropyExt = 0x84FF,
    }

    public enum SyncCondition : int
    {
        SyncGpuCommandsComplete = 0x9117,
    }

    [System.Flags]
    public enum SyncFlags : int
    {
        None = 0,
        FlushCommandsBit = 0x00000001,
    }

    public enum SyncParameterName : int
    {
        ObjectType = 0x9112,
        SyncCondition = 0x9113,
        SyncStatus = 0x9114,
        SyncFlags = 0x9115,
    }

    public enum ClientWaitSyncResult : int
    {
        AlreadySignaled = 0x911A,
        TimeoutExpired = 0x911B,
        ConditionSatisfied = 0x911C,
        WaitFailed = 0x911D,
    }

    public enum TransformFeedbackTarget : int
    {
        TransformFeedback = 0x8E22,
    }

    public enum TransformFeedbackBufferMode : int
    {
        InterleavedAttribs = 0x8C8C,
        SeparateAttribs = 0x8C8D,
    }
}
