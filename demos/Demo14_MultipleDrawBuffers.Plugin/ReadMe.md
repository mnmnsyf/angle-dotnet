## Demo14_MultipleDrawBuffers.Plugin

多渲染目标（MRT）示例，依赖 `GL_EXT_draw_buffers`：先将同一纹理同时输出到 4 个 FBO 颜色附件，再把 4 张贴图分别拷贝到窗口四象限。

### 编译
```powershell
dotnet build demos/Demo14_MultipleDrawBuffers.Plugin/Demo14_MultipleDrawBuffers.Plugin.csproj -c Debug
```

### 运行（WinForms Host）
```powershell
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=14 --show
```

说明：窗口默认 640x480，ES2 上运行；若 `glDrawBuffers/glDrawBuffersEXT` 不可用会直接抛错。
