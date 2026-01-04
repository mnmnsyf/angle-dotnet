## Demo13_TexRedefMicrobench.Plugin

纹理重定义微基准（ES2）：热身 5 帧后，依次输出原纹理 draw、同纹理放大重定义后的定义/绘制耗时，以及新纹理定义/绘制耗时。GLSL 位于 `Shaders/` 可热重载。

### 编译
```powershell
dotnet build demos/Demo13_TexRedefMicrobench.Plugin/Demo13_TexRedefMicrobench.Plugin.csproj -c Debug
```

### 运行（WinForms Host）
```powershell
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=13 --frames=8 --show
```

说明：窗口尺寸 1280x1280，默认 ES2。计时结果打印到控制台。
