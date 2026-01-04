## Demo16_Multiview.Plugin

`GL_OVR_multiview2` 示例：使用 multiview framebuffer 同时渲染两层 `TEXTURE_2D_ARRAY`，再将两层分别拷贝到窗口左右半屏。

### 编译
```powershell
dotnet build demos/Demo16_Multiview.Plugin/Demo16_Multiview.Plugin.csproj -c Debug
```

### 运行（WinForms Host）
```powershell
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=16 --swap-interval=0 --show
```

说明：需 ES3 + `GL_OVR_multiview2`，否则会抛 NotSupportedException；无深度附件，示例仅演示多视图颜色输出。
