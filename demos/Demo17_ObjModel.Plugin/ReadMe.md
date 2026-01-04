## Demo17_ObjModel.Plugin

简单 OBJ 模型加载示例（ES3）：自动加载项目内嵌的 `OPBR_Ace.obj` 模型，使用 CPU 侧解析并进行简单的法线漫反射渲染。

### 编译
```powershell
dotnet build demos/Demo17_ObjModel.Plugin/Demo17_ObjModel.Plugin.csproj -c Debug
```

### 运行（WinForms Host）
```powershell
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=17 --swap-interval=0 --show
```

说明：
- 简化版实现，不再需要命令行参数 `--obj`。
- 默认加载 `OPBR_Ace.obj`。
- 代码经过重构，移除了复杂的矩阵库依赖，使用 `System.Numerics`。
