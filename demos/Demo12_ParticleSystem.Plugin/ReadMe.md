## Demo12_ParticleSystem.Plugin

复刻 ANGLE `particle_system` sample：点精灵粒子 + `gl_PointCoord` 采样纹理，并使用 `SRC_ALPHA, ONE` 做加法混合。

实现参考：
- `external/angle/samples/particle_system/ParticleSystem.cpp`

### 编译
```powershell
dotnet build demos/Demo12_ParticleSystem.Plugin/Demo12_ParticleSystem.Plugin.csproj -c Debug
```

### 运行（WinForms Host）
```powershell
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=12 --show
```

