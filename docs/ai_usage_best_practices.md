## AI 使用 angle-dotnet 最佳实践

- 目标与范围：本仓库提供 .NET 侧 EGL/GLES (ANGLE) SDK 与 WinForms Demo Host。优先保持 CLS 友好，避免引入非必要的新语言特性（C# 9 基线）。

- 入口与分层：
  - 托管绑定：`cs/AngleBridge.Managed`（P/Invoke + GLES 动态加载），新增 GL 入口点放在 `Gles.cs` 与 `GlesLoader.cs`，不要直接暴露到 public API。
  - SDK 层：`cs/AngleBridge.Sdk`（CLS 友好的 `AngleDevice/AngleGl` 等），新增功能先在这里封装，验证线程亲和与资源归属。`AngleGl` 采用 Facade 模式，功能按模块（State/Draw/Textures 等）组织。
  - Utilities：`cs/AngleBridge.Utilities`（TGA 加载、纹理工具、WinForms Host CLI），可复用而不影响核心层。

- Shader 与热重载：
  - 插件 Demo 的 GLSL 放在各自 `Shaders/` 目录，并在项目文件中 `CopyToOutputDirectory=PreserveNewest`。
  - 通过 `ShaderProgramReloader` 热更，Renderer 实现 `IRendererWithHostContext`，在 `SetHostContext` 构造 reloader；更新 program 后重新绑定 attrib/uniform。
  - 修改运行中的 shader 会自动重载，错误会输出到控制台。

- 运行与调试命令：
  - 构建 Host：`dotnet build demos/DemoHost.WinForms/DemoHost.WinForms.csproj -c Debug`
  - 运行示例：`dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=<n> --show`
    - Demo7 (tri_fan_microbench)：`--frames=300 --swap-interval=0`，结束时输出表格与 CSV。
    - Demo8/9 纹理参数（透传）：`--tex=...` 或 `--tex0/--tex1=...`（仅 TGA），`--tex-wrap-preset=repeat|clamp|mirror`，`--tex-wrap-uv`（放大 UV），`--tex-min/--tex-mag`，`--tex-mipmap/--tex-no-mipmap`。

- 纹理与资源：
  - 内置解码仅支持 TGA（`AngleBridge.Utilities.TgaImage`/`TextureUtils`），PNG/JPG 需外部解码后用 `glTexImage2D` 上传。
  - 上传前设置 `PixelStore(UNPACK_ALIGNMENT,1)` 以避免对齐问题。

- GL API 扩展原则：
  - 先在 `Gles.cs` 添加常量/委托，再在 `GlesLoader.cs` 加载。
  - 在 `AngleGl` 的对应子模块（如 `AngleGl.StateApi`, `AngleGl.DrawApi`）暴露 CLS 友好方法，通过 `gl.State.Method()` 调用。
  - 避免直接在 `AngleGl` 根类添加新方法（旧方法已标记 Obsolete）。
  - 枚举放 `cs/AngleBridge.Sdk/Interop/GlEnums.cs`，保持 `enum : int`，公开 API 避免 `uint` 暴露。
  - 公共资源类型（Buffer/Texture/Program 等）用封装类，校验设备归属并支持多次 Dispose 安全。

- Demo 开发约束：
  - 保持编号连续（DemoN_*），csproj 记得复制 Shaders/资源，并在 Host Program.cs 注册 `--demo=N`。
  - 使用新 API 风格：`gl.State.Viewport(...)`, `gl.Textures.BindTexture(...)`。
  - 默认用 ES3（除特殊 ES2/ES1 示例），窗口尺寸/可见性按需求设置.
  - 运行参数如需透传，通过 WinFormsHostCli 容忍，而实际解析放在插件内部（Environment.GetCommandLineArgs）。
  - 实现 ANGLE samples 时先对照 `external/angle/samples/<sample>/` 与 `external/angle/samples/sample_util/`，尽量保持 shader/顶点数据/纹理生成逻辑一致，便于肉眼与脚本对比验证。
  - 保持精简：优先做“最小可对比版本”，避免无关参数、过度抽象与额外依赖；缺入口点时按“绑定层→SDK 层→Demo”最小增量补齐。
  - 每个Demo都需要写 ReadMe，说明目标、编译与运行命令。

- 性能与输出：
  - 需要基准时（如 Demo7）同时记录 CPU/ GPU，GPU 计时使用 timer query + disjoint 检测；输出表格并落盘 CSV（`out/<demo>/timestamp.csv`）。
  - 禁止在 hot path 频繁分配或写日志；必要时在结束时汇总输出。

- 常见坑：
  - 纹理加载失败：确认 TGA 路径/格式正确，wrap/filter 参数合法。
  - Shader 修改后位置绑定变化：重载后必须重新获取 attrib/uniform。
  - 线程亲和：所有 AngleDevice/AngleGl 调用必须在创建线程执行。
  - 缺失 GL 入口：若报 NotLoaded/NotSupported，检查是否在 GlesLoader 加载、是否需要 ES3/扩展。
  - GL Error：如报 "Program object expected"，检查是否忘记 `UseProgram`。
