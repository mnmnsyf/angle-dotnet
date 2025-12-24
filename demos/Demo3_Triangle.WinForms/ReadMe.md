## Demo3_Triangle.WinForms

WinForms 上屏渲染：创建窗口 EGL/GLES 上下文、启动渲染循环绘制三角形，并支持 ALC 插件热重载。

### 编译与运行（Windows）

1. 先编译 native（生成带 `AB_CreateWindowContext` 的 `AngleBridge.dll`）

```powershell
cmake --build --preset build-debug --target AngleBridge 
```

2. 启动 WinForms 宿主

```powershell
dotnet run -c Debug --project demos/Demo3_Triangle.WinForms/Demo3_Triangle.WinForms.csproj 
```

### 验证热重载
改插件里片元着色器颜色 `FragColor`（例如改成蓝色），然后只重新编译插件：

```powershell
dotnet build demos/Demo3_Triangle.Plugin/Demo3_Triangle.Plugin.csproj -c Debug 
```

窗口会自动 reload，新颜色即生效。

补充说明：Release 同理，先构建 `cmake --build --preset build-release --target AngleBridge`，然后 `dotnet run -c Release ...`
