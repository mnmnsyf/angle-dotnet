## demo2_csharp

最小 C# 验证：创建离屏 EGL/GLES 上下文并输出 `GL_VERSION`。

### 编译 / 运行（Windows）
```powershell
dotnet run -c Debug --project demos/demo2_csharp/demo2_csharp.csproj
```

说明：如果输出目录缺少 `AngleBridge.dll` 或 ANGLE 运行时 DLL，请先按仓库说明构建 native（`AngleBridge` 目标），再重新运行。

