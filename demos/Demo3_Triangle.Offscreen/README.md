## Demo3_Triangle.Offscreen

C# 离屏渲染：复用 `Demo3_Triangle.Plugin` 的渲染器画一帧三角形，`ReadPixels` 输出 PPM 图片（P6）。

### 运行（Windows）
```powershell
dotnet run -c Debug --project demos/Demo3_Triangle.Offscreen/Demo3_Triangle.Offscreen.csproj -- --out scripts/out/demo3_csharp.ppm --w 256 --h 256
```

### 与 C++ 结果对比
先运行 `demo3_native` 生成 `demo3_cpp.ppm`，再执行：
```powershell
python scripts/compare_PPM.py scripts/out/demo3_cpp.ppm scripts/out/demo3_csharp.ppm
```

### 性能基准测试 (Benchmark)
使用自动脚本一键对比 C# 与 C++ 的性能：
```powershell
python scripts/benchmark.py --work 100 --frames 1000
```
结果将输出到 `out/benchmark/summary.csv`。
