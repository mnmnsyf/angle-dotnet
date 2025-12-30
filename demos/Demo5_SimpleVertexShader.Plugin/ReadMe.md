## Demo5_SimpleVertexShader.Plugin

简单顶点/片元着色器示例（ES3，支持 Shader 热重载，GLSL 位于 `Shaders/`）。

### 编译
```powershell
dotnet build demos/Demo5_SimpleVertexShader.Plugin/Demo5_SimpleVertexShader.Plugin.csproj -c Debug
```

### 运行（WinForms Host）
```powershell
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=5 --show
```
