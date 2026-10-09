# ETW Profiling

This guide describes a Windows profiling workflow for angle-dotnet.

## Tools

- Windows Performance Recorder (WPR)
- Windows Performance Analyzer (WPA)
- PerfView
- .NET counters and `dotnet-trace`

## Workflow

1. Build the Release configuration.
2. Run a deterministic demo with a fixed resolution and workload.
3. Capture a short ETW trace containing CPU scheduling, GPU, DirectX, and .NET events.
4. Open the trace in WPA and compare CPU submission, render-thread, and GPU intervals.
5. Record the command line, driver, ANGLE backend, resolution, and frame count with the result.

Avoid drawing conclusions from a single warm-up frame. Use a warm-up period and report the measured interval separately.
