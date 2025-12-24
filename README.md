# angle-dotnet

基于 **ANGLE** (EGL + GLES) 构建的轻量级 **.NET/C#** OpenGL ES 封装库。

## 目标
- 提供可复用的 .NET 库，通过 ANGLE 创建 OpenGL ES 上下文
- 保持跨平台兼容性
- 通过 Demos 验证功能进展

## 架构分层

- **`cs/AngleBridge.Managed` (绑定层)**
  - 负责 P/Invoke 调用 `AngleBridge.dll` 及动态加载 GLES 入口点。
  - 属于内部实现细节。

- **`cs/AngleBridge.Sdk` (SDK 层)**
  - **推荐使用的公开 API**，符合 CLS 规范。
  - `AngleDevice`: 管理 ANGLE/EGL 上下文及窗口/离屏表面，强制线程亲和性。
  - `AngleGl`: 提供面向对象的 GLES 接口封装。
  - 资源管理: `AngleShader`, `AngleProgram`, `AngleBuffer` 等实现了 `IDisposable`，自动管理 GPU 资源生命周期。

## 快速上手 (离屏渲染)

```csharp
using AngleBridge.Sdk.Runtime;

// 创建离屏上下文 (256x256, GLES 3.0)
using (var device = AngleDevice.CreateOffscreen(256, 256, new AngleDeviceOptions { GlesMajor = 3 }))
{
    var gl = device.Gl;
    
    // 调用 GLES 绘制...
    gl.ClearColor(1.0f, 0.0f, 0.0f, 1.0f);
    gl.Clear(ClearBufferMask.ColorBufferBit);
    
    gl.Finish();

    // 读取像素
    var rgba = new byte[256 * 256 * 4];
    device.ReadPixelsRgba(0, 0, 256, 256, rgba, 0);
}
```

## 性能基准测试与调试

### 基准测试 (Benchmark)
本项目包含自动化脚本，用于对比 Native C++ 实现与 Managed C# 封装的性能差异。

```bash
python scripts/benchmark.py --work 100 --frames 1000
```

该命令将：
1. 自动以 Release 模式构建 C++ 和 C# 项目。
2. 运行基准测试并生成对比报告（CPU 提交耗时、帧耗时、GPU 耗时）。
3. 结果输出至 `test/benchmark/summary.csv`。

### 性能火焰图 (Profiling)
支持使用 `dotnet-trace` 对 C# 封装层进行性能分析，并生成 Speedscope 格式的火焰图。

1. 确保已安装工具：
   ```bash
   dotnet tool install --global dotnet-trace
   ```
2. 运行 Profiling 模式：
   ```bash
   python scripts/benchmark.py --profile
   ```
3. 分析结果：
   - 脚本将生成 `test/benchmark/trace.speedscope.json`。
   - 将该文件上传至 [speedscope.app](https://www.speedscope.app/) 即可查看交互式火焰图。
