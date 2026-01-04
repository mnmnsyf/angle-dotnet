## Demo15_SimpleInstancing.Plugin

`GL_ANGLE_instanced_arrays` 示例：将小四边形实例化平铺在屏幕上，演示 `glVertexAttribDivisorANGLE` + `glDrawElementsInstancedANGLE`。

### 编译
```powershell
dotnet build demos/Demo15_SimpleInstancing.Plugin/Demo15_SimpleInstancing.Plugin.csproj -c Debug
```

### 运行（WinForms Host）
```powershell
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=15 --swap-interval=0 --show
```

说明：默认 ES2，需支持 `GL_ANGLE_instanced_arrays`，否则会抛出 NotSupportedException。
