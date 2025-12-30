## Demo7_TriFanMicrobench.Plugin

TRIANGLE_FAN vs TRIANGLES 微基准（ES2），输出 CPU/GPU 计时表格和 CSV，GLSL 位于 `Shaders/` 支持热重载。

### 编译
```powershell
dotnet build demos/Demo7_TriFanMicrobench.Plugin/Demo7_TriFanMicrobench.Plugin.csproj -c Debug
```

### 运行（WinForms Host）
```powershell
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=7 --frames=300 --swap-interval=0
```

说明：结束时打印表格并写入 `out/tri_fan_microbench/*.csv`。
