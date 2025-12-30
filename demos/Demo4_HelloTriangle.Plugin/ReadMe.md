## Demo4_HelloTriangle.Plugin

最小 Hello Triangle 插件（ES2），供 WinForms 宿主加载。

### 编译
```powershell
dotnet build demos/Demo4_HelloTriangle.Plugin/Demo4_HelloTriangle.Plugin.csproj -c Debug
```

### 运行（WinForms Host）
```powershell
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=4 --show
```
