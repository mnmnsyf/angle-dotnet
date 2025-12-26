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
        Triangles = 0x0004,
    }

    public enum BufferTarget : int
    {
        ArrayBuffer = 0x8892,
        ElementArrayBuffer = 0x8893,
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
        CullFace = 0x0B44,
        DepthTest = 0x0B71,
        StencilTest = 0x0B90,
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
    }

    public enum TextureParameterName : int
    {
        TextureMinFilter = 0x2801,
        TextureMagFilter = 0x2800,
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
}
