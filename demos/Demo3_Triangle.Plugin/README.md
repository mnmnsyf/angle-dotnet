## Demo3_Triangle.Plugin

Demo3 的渲染器插件（实现共享接口 `IRenderer`），用于 WinForms 热重载与离屏渲染复用。

### 编译（Windows）
```powershell
dotnet build demos/Demo3_Triangle.Plugin/Demo3_Triangle.Plugin.csproj -c Debug
```

说明：修改 `TriangleRenderer` 后重新编译该项目，WinForms 宿主会自动热重载。

