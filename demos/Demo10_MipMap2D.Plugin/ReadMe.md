## Demo10_MipMap2D.Plugin

复刻 ANGLE `mip_map_2d` sample：使用同一张 mipmap 纹理绘制 3 个 quad，用于对比不同过滤方式的效果差异（GLSL 位于 `Shaders/`，支持热重载）。

- 左：`GL_NEAREST`
- 中：`GL_LINEAR_MIPMAP_LINEAR`
- 右：`GL_LINEAR_MIPMAP_LINEAR` + `GL_TEXTURE_MAX_ANISOTROPY_EXT`（若支持则使用最大值，否则与中间一致）

实现参考：
- `external/angle/samples/mip_map_2d/MipMap2D.cpp`
- `external/angle/samples/sample_util/texture_utils.cpp`（`CreateMipMappedTexture2D()`）

### 编译
```powershell
dotnet build demos/Demo10_MipMap2D.Plugin/Demo10_MipMap2D.Plugin.csproj -c Debug
```

### 运行（WinForms Host）
```powershell
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=10 --show
```
