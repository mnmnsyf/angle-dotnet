## Demo18_PostSubBuffer.Plugin

复刻 ANGLE `post_sub_buffer` 示例：先全屏清蓝，再用 `eglPostSubBufferNV` 只更新中间矩形，保留外圈蓝色。

### 编译
```powershell
dotnet build demos/Demo18_PostSubBuffer.Plugin/Demo18_PostSubBuffer.Plugin.csproj -c Debug
```

### 运行（WinForms Host）
```powershell
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=18 --show
```
