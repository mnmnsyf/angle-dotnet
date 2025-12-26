# angle-dotnet

基于 **ANGLE**（EGL + GLES）构建的 **.NET/C# OpenGL ES SDK** 仓库，用于在 .NET 中创建并使用 OpenGL ES 上下文（上屏/离屏）。

## 目标
- 提供可复用的 .NET 库：通过 ANGLE 创建 OpenGL ES 上下文并进行渲染
- 支持 **window surface（可 swap/present）** 与 **offscreen（可 readback）** 两条路径
- 保持跨平台方向（取决于 ANGLE 支持的平台）
- 以 demo 驱动：每个里程碑配套 demo/可对比输出用于验证行为

## 托管层分层（建议用法）

- `cs/AngleBridge.Managed`（绑定层）
  - P/Invoke 调用 `AngleBridge.dll` + 动态加载 GLES 入口（`GlesLoader` + `Gles`）。
  - 不以 CLS 友好为目标，视为内部实现细节。
- `cs/AngleBridge.Sdk`（SDK 层，CLS 友好）
  - `[assembly: CLSCompliant(true)]`
  - `AngleDevice`：持有 ANGLE/EGL context + surface（window/offscreen），强制线程亲和性，提供 present/readback。
  - `AngleGl`：上下文绑定的 GLES facade，使用 `enum : int` + `int` 句柄包装（避免在 public API 暴露 `uint`）。
  - 资源对象：`AngleShader` / `AngleProgram` / `AngleBuffer` / `AngleVertexArray` / `AngleTexture`（均为 `IDisposable`，不暴露 raw handle）。
- `cs/AngleBridge.Utilities`（可选）
  - sample runner 框架（`SampleApplication`）+ 窗口/事件抽象（`ISampleWindow`）。
  - TGA/纹理辅助工具（供 samples 复用）。
- `cs/AngleBridge.Utilities/WinForms`（Windows 可选实现）
  - `WinFormsSampleWindow`：`ISampleWindow` 的 WinForms 实现。
  - `WinFormsHotReloadHost` + `WinFormsHostCli`：可复用 WinForms Host + CLI，用于加载可热重载的 renderer 插件。

## 线程模型

`AngleDevice` 是 **线程亲和** 的：所有 `AngleDevice`/`AngleGl` 调用必须发生在创建它的线程上（EGL context 规则）。

## 快速上手（离屏）

```csharp
using AngleBridge.Sdk.Runtime;

using (var device = AngleDevice.CreateOffscreen(256, 256, new AngleDeviceOptions { GlesMajor = 3 }))
{
    var gl = device.Gl;
    // 渲染...
    gl.Finish();

    var rgba = new byte[256 * 256 * 4];
    device.ReadPixelsRgba(0, 0, 256, 256, rgba, 0);
}
```

## 运行 Demo（WinForms Host）

```powershell
# 启动 DemoHost（示例：Demo5 simple_vertex_shader）
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=5 --show

# 选择 ANGLE 后端/设备/交换间隔（示例）
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=5 --show --use-angle=d3d11 --swap-interval=0
```

## 性能分析
- ETW（WPA/PerfView）：`docs/profiling_etw.md`
