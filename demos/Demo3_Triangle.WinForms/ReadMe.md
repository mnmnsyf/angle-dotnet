运行和验证热重载:

1. 先编译 native（生成带 AB_CreateWindowContext 的 AngleBridge.dll）

```powershell
cmake --build --preset build-debug --target AngleBridge 
```

2. 启动 WinForms 宿主

```powershell
dotnet run -c Debug --project demos/Demo3_Triangle.WinForms/Demo3_Triangle.WinForms.csproj 
```

3. 验证热重载
   改 TriangleRenderer.cs 里片元着色器颜色FragColor（比如改成蓝色），然后只重新编译插件：

```powershell
dotnet build demos/Demo3_Triangle.Plugin/Demo3_Triangle.Plugin.csproj -c Debug 
```

窗口会在1秒内自动 reload，新颜色即生效。

补充说明
- Release 同理，先构建 cmake --build --preset build-release --target AngleBridge，然后用 -c Release启动
