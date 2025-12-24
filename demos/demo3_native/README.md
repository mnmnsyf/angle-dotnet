## demo3_native

纯 C++/ANGLE 离屏渲染：画一个与 Demo3 相同的三角形，并输出 PPM 图片（P6）。

### 编译（Windows）
```powershell
cmake --build --preset build-debug --target demo3_native
```

### 运行（Windows）
```powershell
build/vs2022-debug/demos/demo3_native/Debug/demo3_native.exe --out scripts/out/demo3_cpp.ppm --w 256 --h 256
```

### 与 C# 结果对比
先运行 `Demo3_Triangle.Offscreen` 生成 `demo3_csharp.ppm`，再执行：
```powershell
python scripts/compare_PPM.py scripts/out/demo3_cpp.ppm scripts/out/demo3_csharp.ppm
```

### 性能基准测试 (Benchmark)
推荐使用根目录下的自动脚本：
```powershell
python scripts/benchmark.py --work 100 --frames 1000
```
该脚本会自动编译 Release 版本并生成对比报告。
