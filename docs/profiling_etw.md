# 基于 ETW 的性能分析 (WPA/PerfView)

本文档介绍如何在 Windows 环境下使用 **ETW (Event Tracing for Windows)** 对项目进行深度的系统级性能分析。ETW 相比 `dotnet-trace` 的优势在于它可以同时捕获内核事件（上下文切换、磁盘 I/O）、原生代码（C++ / 驱动）和托管代码（C# JIT / GC）的混合调用栈。

## 方案一：PerfView (推荐)

PerfView 是微软官方提供的轻量级 ETW 采集与分析工具，**无需安装**，适合快速排查问题。

### 1. 下载工具
从 GitHub Release 页面下载 `PerfView.exe`：
[https://github.com/microsoft/perfview/releases](https://github.com/microsoft/perfview/releases)

### 2. 采集 Trace

1. 以**管理员身份**运行 `PerfView.exe`。
2. 点击菜单栏 **Collect -> Collect** (快捷键 `Alt+C`)。
3. 在弹出窗口中配置：
   - **Data File**: 设置输出路径（例如 `D:\Code\angle-dotnet\trace.etl`）。
   - **Zip**: 取消勾选（方便直接用 WPA 打开）。
   - **Merge**: 勾选（合并符号信息）。
   - **Advanced Options**:
     - **Kernel Events**: 确保勾选 `Context Switch`, `CPU Samples`。
     - **.NET Events**: 默认即可。
4. 点击 **Start Collection** 开始录制。
5. 运行你的测试程序（例如 `Demo3`）：
   ```powershell
   dotnet run -c Release --project demos/Demo3_Triangle.Offscreen/Demo3_Triangle.Offscreen.csproj
   ```
6. 程序运行结束后，在 PerfView 点击 **Stop Collection**。
7. 等待合并完成（Status 栏显示 Ready）。

### 3. 分析 Trace

PerfView 内置了强大的分析功能，或者你可以用 WPA 打开生成的 `.etl` 文件。

#### 在 PerfView 中查看混合调用栈
1. 在左侧文件树双击生成的 `.etl` 文件。
2. 双击 **CPU Stacks**。
3. 在 **Process** 过滤器中输入 `Demo3` 或 `dotnet` 找到你的进程。
4. 展开堆栈，你将能看到从 C# (`AngleBridge.Sdk`) -> P/Invoke (Stub) -> C++ (`AngleBridge.dll`) -> 驱动 (`nvoglv64.dll` 等) 的完整调用链。

---

## 方案二：Windows Performance Recorder (WPR) + Analyzer (WPA)

WPA 提供了更现代化的 UI 和强大的图形化分析能力，适合分析线程调度、GPU 占用等。

### 1. 安装工具
WPA 包含在 **Windows Assessment and Deployment Kit (ADK)** 中。
下载地址：[Windows ADK](https://learn.microsoft.com/en-us/windows-hardware/get-started/adk-install)
*安装时只需勾选 "Windows Performance Toolkit"*。

### 2. 编写自定义 Profile (可选)
为了更好地捕捉 .NET 和 ANGLE 的混合数据，推荐使用以下 `.wprp` 配置文件：

创建一个文件 `AngleDotnet.wprp`：
```xml
<?xml version="1.0" encoding="utf-8"?>
<WindowsPerformanceRecorder Version="1.0">
  <Profiles>
    <SystemCollector Id="SystemCollector_Base" Name="System Collector">
      <BufferSize Value="1024" />
      <Buffers Value="64" />
    </SystemCollector>

    <EventCollector Id="EventCollector_User" Name="User Collector">
      <BufferSize Value="1024" />
      <Buffers Value="64" />
    </EventCollector>

    <Profile Id="AngleDotnetProfile.Verbose.File" Name="AngleDotnetProfile" Description="Tracing for ANGLE .NET" LoggingMode="File" DetailLevel="Verbose">
      <Collectors>
        <SystemCollectorId Value="SystemCollector_Base">
          <SystemProviderId Value="SystemProvider_Base" />
        </SystemCollectorId>
        <EventCollectorId Value="EventCollector_User">
          <EventProviders>
            <!-- Microsoft-Windows-DotNETRuntime -->
            <EventProviderId Value="EventProvider_DotNet" />
            <!-- ANGLE (如果未来添加了 ETW Provider) -->
          </EventProviders>
        </EventCollectorId>
      </Collectors>
    </Profile>

    <SystemProvider Id="SystemProvider_Base">
      <Keywords>
        <Keyword Value="CpuConfig" />
        <Keyword Value="Loader" />
        <Keyword Value="ProcessThread" />
        <Keyword Value="SampledProfile" />
        <Keyword Value="ContextSwitch" />
        <Keyword Value="ReadyThread" />
        <Keyword Value="DiskIO" />
        <Keyword Value="HardFaults" />
      </Keywords>
      <Stacks>
        <Stack Value="SampledProfile" />
        <Stack Value="ReadyThread" />
        <Stack Value="CSwitch" />
      </Stacks>
    </SystemProvider>

    <EventProvider Id="EventProvider_DotNet" Name="e13c0d23-ccbc-4e12-931b-d9cc2eee27e4">
      <Keywords>
        <Keyword Value="0x98" /> <!-- JIT + Loader + NGen -->
      </Keywords>
      <CaptureStateOnStart>
        <Keyword Value="0x98" />
      </CaptureStateOnStart>
    </EventProvider>
  </Profiles>
</WindowsPerformanceRecorder>
```

### 3. 采集 Trace
使用命令行工具 `wpr`：

```powershell
# 开始录制 (使用默认配置或自定义 wprp)
wpr -start AngleDotnet.wprp

# 运行你的程序
dotnet run -c Release ...

# 停止并保存
wpr -stop trace.etl
```

### 4. 使用 WPA 分析
1. 打开 `Windows Performance Analyzer`。
2. 菜单 **File -> Open** 打开 `trace.etl`。
3. **加载符号 (Symbols)**:
   - 菜单 **Trace -> Load Symbols**。
   - 确保设置了符号路径（包含微软服务器和本地构建路径）：
     `srv*https://msdl.microsoft.com/download/symbols;D:\Code\angle-dotnet\cs\AngleBridge.Managed\bin\Release\net9.0;D:\Code\angle-dotnet\build\vs2022-release\demos\demo3_native\Release`
4. **关键图表**:
   - **Computation -> CPU Usage (Sampled)**: 查看 CPU 消耗热点。右键选择 `Stack` 列，确保能看到托管到原生的转换。
   - **System Activity -> Generic Events**: 如果有自定义 ETW 事件。

## 常见问题

**Q: 为什么看不到 C# 函数名？**
A: 确保在采集时开启了 .NET Runtime Provider (PerfView 默认开启)。如果在 WPA 中，需要确保加载了 NGEN/R2R 符号，或者在 PerfView 中先执行 "Merge" 操作，它会生成 `.pdb` 辅助 WPA 解析 JIT 代码。

**Q: 如何查看 P/Invoke 开销？**
A: 在 CPU 堆栈中寻找 `System.Private.CoreLib!PInvokeStub_...` 类似的帧。如果该帧耗时显著，说明跨语言调用过于频繁。

**Q: 线程亲和性验证**
A: 在 WPA 中使用 **Context Switch** 图表，按 `ThreadId` 分组，观察特定线程是否一直在同一个 CPU Core 上运行（如果绑定了核心），或者是否被频繁抢占。
