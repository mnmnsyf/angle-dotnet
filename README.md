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

## 性能基准测试

本项目包含自动化脚本，用于对比 Native C++ 实现与 Managed C# 封装的性能差异。

```bash
python scripts/benchmark.py --work 100 --frames 1000
```

该命令将：
1. 自动以 Release 模式构建 C++ 和 C# 项目。
2. 运行基准测试并生成对比报告（CPU 提交耗时、帧耗时、GPU 耗时）。
3. 结果输出至 `out/benchmark/summary.csv`。
