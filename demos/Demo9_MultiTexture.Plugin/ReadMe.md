## Demo9_MultiTexture.Plugin

多纹理采样示例（ES3，两张纹理叠加，GLSL 位于 `Shaders/`，支持热重载）。默认用内置 2×2 RGBA 纹理，可加载 TGA。

### 编译
```powershell
dotnet build demos/Demo9_MultiTexture.Plugin/Demo9_MultiTexture.Plugin.csproj -c Debug
```

### 运行（WinForms Host）
```powershell
# 使用内置纹理
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=9 --show

# 指定两张 TGA 纹理 + wrap/filter
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=9 --show --tex0=path/to/basemap.tga --tex1=path/to/lightmap.tga --tex-wrap-preset=repeat --tex-min=linear_mipmap_linear --tex-mag=linear
```
