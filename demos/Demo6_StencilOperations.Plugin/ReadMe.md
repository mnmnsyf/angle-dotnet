## Demo6_StencilOperations.Plugin

模板+深度测试组合示例（ES3，GLSL 位于 `Shaders/`，支持热重载）。

### 编译
```powershell
dotnet build demos/Demo6_StencilOperations.Plugin/Demo6_StencilOperations.Plugin.csproj -c Debug
```

### 运行（WinForms Host）
```powershell
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=6 --show
```
