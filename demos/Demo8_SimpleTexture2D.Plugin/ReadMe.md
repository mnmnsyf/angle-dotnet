## Demo8_SimpleTexture2D.Plugin

最基础 2D 纹理 + wrap 演示（ES3，GLSL 位于 `Shaders/`，支持热重载）。默认使用内置 2×2 RGB，可通过命令行加载 TGA。

### 编译
```powershell
dotnet build demos/Demo8_SimpleTexture2D.Plugin/Demo8_SimpleTexture2D.Plugin.csproj -c Debug
```

### 运行（WinForms Host）
```powershell
# 内置纹理
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=8 --show

# 使用外部 TGA
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=8 --show --tex=demos/Demo8_SimpleTexture2D.Plugin/red_brick_diff_4k.tga

# 仅在需要观察 wrap 时放大 UV（默认关闭）
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=8 --show --tex-wrap-uv --tex-wrap-preset=repeat --tex-min=linear --tex-mag=linear
```
