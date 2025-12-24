namespace AngleBridge.Sdk.Interop
{
    [System.Flags]
    public enum ClearBufferMask : int
    {
        ColorBufferBit = 0x00004000,
    }

    public enum PrimitiveType : int
    {
        Triangles = 0x0004,
    }

    public enum BufferTarget : int
    {
        ArrayBuffer = 0x8892,
    }

    public enum BufferUsage : int
    {
        StaticDraw = 0x88E4,
    }

    public enum VertexAttribType : int
    {
        Float = 0x1406,
    }

    public enum ShaderType : int
    {
        VertexShader = 0x8B31,
        FragmentShader = 0x8B30,
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
}

