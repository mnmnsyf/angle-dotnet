namespace AngleBridge.Sdk.Runtime
{
    public sealed class AngleDeviceOptions
    {
        public int GlesMajor { get; set; } = 3;
        public AngleBackend Backend { get; set; } = AngleBackend.Default;
        public AngleDeviceType DeviceType { get; set; } = AngleDeviceType.Default;
        public int SwapInterval { get; set; } = -1;
    }
}
