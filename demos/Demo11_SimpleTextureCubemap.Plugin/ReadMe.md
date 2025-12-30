## Demo11_SimpleTextureCubemap.Plugin

复刻 ANGLE `simple_texture_cubemap` sample：在球体上用 `samplerCube` 按法线方向采样 cubemap。

实现参考：
- `external/angle/samples/simple_texture_cubemap/SimpleTextureCubemap.cpp`
- `external/angle/samples/sample_util/texture_utils.cpp`（`CreateSimpleTextureCubemap()`）

### 编译
```powershell
dotnet build demos/Demo11_SimpleTextureCubemap.Plugin/Demo11_SimpleTextureCubemap.Plugin.csproj -c Debug
```

### 运行（WinForms Host）
```powershell
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=11 --show
```
