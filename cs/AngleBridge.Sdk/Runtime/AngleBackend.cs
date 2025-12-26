namespace AngleBridge.Sdk.Runtime
{
    public enum AngleBackend : int
    {
        Default = 0,
        D3D11 = 1,
        D3D9 = 2,
        Vulkan = 3,
        OpenGL = 4,
        OpenGles = 5,
        Metal = 6,
        Null = 7,
    }

    public enum AngleDeviceType : int
    {
        Default = 0,
        Hardware = 1,
        Warp = 2,
        Reference = 3,
        SwiftShader = 4,
        Null = 5,
    }
}
