# ANGLE samples 覆盖矩阵（external/angle/samples）

目标：为了“托管所有 ANGLE 暴露的方法”，最终验证方式是能跑通 `external/angle/samples` 下所有示例。本文档按目录盘点每个 sample 的关键能力、需要的 EGL/GL 功能，并对照当前 `angle-dotnet` 的能力标注缺口，最后一列用于完成后打勾。

当前仓库能力基线（用于“当前缺口”对照）：

- Native Bridge：目前以 `AB_Handle` 封装单个 `EGLDisplay/EGLContext/EGLSurface`，支持创建单个 offscreen(pbuffer)/window surface 上下文、`SwapBuffers`、`eglGetProcAddress`；未暴露多 surface / 多 context / share context / MakeCurrent 切换等通用 EGL 原语。
- Managed：`Gles` 仅覆盖 Demo3 所需的少量 GLES2/3 入口（shader/program、少量 buffer/VAO、draw/clear/viewport/readback、EXT_disjoint_timer_query）；纹理/FBO/MRT/instancing/stencil/depth/sync/ES3 纹理数组等未系统覆盖。

下表已按“实现优先级（最简 → 最难）”排序（基于上面的仓库能力基线与增量建设成本）。

| Sample(目录) | 关键能力 | 需要的 EGL 能力 | 需要的 GL/GLES 能力 | 当前缺口（相对本仓库） | 完成 |
|---|---|---|---|---|---|
| `hello_triangle` | 最小可运行的三角形（上屏） | 单 window surface + swap（可关 vsync） | GLES2：shader compile/link/use、attrib array、`glDrawArrays` | GL 基础大体已具备；仍缺“通用 sample runner/窗口抽象”（目前只有 Demo/WinForms 形态） | [ ] |
| `sample_util` | samples 通用框架（OSWindow/EGLWindow/loader/debug） | EGLWindow：display/config/context/surface 管理；swap interval；平台参数（`EGL_PLATFORM_ANGLE_*`） | 自动 GL loader、debug callback（`GL_KHR_debug`）等 | 需要在托管侧补一个等价的“SampleRunner/Windowing/EGL 配置”层（或以 WinForms/Offscreen runner 逐步替代） | [ ] |
| `simple_vertex_shader` | 基础 vertex shader + MVP 矩阵 | 常规上下文 | uniform matrix（`glUniformMatrix4fv`）、cull face | 托管 `Gles` 当前缺 `glUniformMatrix4fv`/cull face 等；需要基础 math（矩阵）与几何数据管理 | [ ] |
| `stencil_operations` | stencil + depth 测试组合演示 | 需要带 depth/stencil 的 EGLConfig | `glClearStencil/glStencilFunc/glStencilOp/glStencilMask`；`glEnable(GL_STENCIL_TEST/GL_DEPTH_TEST)` | Native Bridge `chooseConfig` 虽指定 depth/stencil，但托管 `Gles` 缺 stencil/depth 相关入口与常量；需补齐 | [ ] |
| `tri_fan_microbench` | TRIANGLE_FAN vs TRIANGLES microbench（VBO） | 常规上下文 | VBO：`glGenBuffers/glBindBuffer/glBufferData`；`glDrawArrays` | 托管 `Gles` 已有部分 VBO/VAO，但缺更完整 buffer API（不同数据类型/usage 等）；若要做 microbench 需统一计时输出 | [ ] |
| `simple_texture_2d` | 最基础 2D 纹理采样 | 常规上下文 | 2D 纹理上传 + sampler；`glActiveTexture` | 托管 `Gles` 缺纹理 API；SDK 层缺纹理对象封装 | [ ] |
| `texture_wrap` | wrap 模式对比（repeat/clamp/mirror） | 常规上下文 | `glTexParameteri(GL_TEXTURE_WRAP_S/T, …)` + 2D 纹理 | 托管 `Gles` 缺纹理 API（wrap 设置等） | [ ] |
| `multi_texture` | 多纹理单次 draw（多纹理单元） | 常规窗口/离屏上下文 | 纹理加载/上传、`glActiveTexture`、多 sampler uniform | 托管 `Gles` 缺纹理 API（`GenTextures/TexImage2D/TexParameteri` 等）+ 资源加载（TGA）复用层 | [ ] |
| `mip_map_2d` | mipmap + 各向异性过滤演示 | 常规窗口/离屏上下文 | 纹理创建与 mipmap 级别、`glGetFloatv(GL_MAX_TEXTURE_MAX_ANISOTROPY_EXT)`、`glTexParameterf(GL_TEXTURE_MAX_ANISOTROPY_EXT, …)` | 托管 `Gles` 缺纹理 API、`glGetFloatv/glTexParameterf` 及各向异性常量；SDK 层缺纹理资源封装 | [ ] |
| `simple_texture_cubemap` | cubemap 纹理采样（球体） | 常规上下文 | cubemap 纹理（`GL_TEXTURE_CUBE_MAP`）、几何数据 draw elements | 托管 `Gles` 缺 cubemap 纹理 API；需要几何工具/资源加载策略 | [ ] |
| `particle_system` | 点精灵粒子 + 混合 + 纹理采样 | 常规窗口/离屏上下文 | GLES2：`GL_POINTS`、`gl_PointCoord`、blend（`glEnable(GL_BLEND)/glBlendFunc`）、2D 纹理 | 托管 `Gles` 缺纹理与混合相关 API；需要可复用的纹理加载（TGA） | [ ] |
| `tex_redef_microbench` | 纹理重定义 microbench（反复 `glTexImage2D`） | 常规上下文 | 纹理 API（`glTexImage2D` 多次）、基础 draw | 托管 `Gles` 缺纹理 API；若要做可比 microbench 还需统一计时与输出 | [ ] |
| `multiple_draw_buffers` | MRT（多渲染目标）+ FBO 贴图回显 | 常规上下文；需能通过 `eglGetProcAddress` 加载扩展入口 | FBO/纹理附件；`glDrawBuffers` 或 `glDrawBuffersEXT`（`GL_EXT_draw_buffers`）；多个 color attachment | 托管 `Gles` 缺 FBO/纹理 API；缺 `glDrawBuffers(EXT)` 扩展加载与常量；SDK 层缺 FBO/RT 封装 | [ ] |
| `simple_instancing` | `GL_ANGLE_instanced_arrays` 实例化绘制 | 常规上下文；通过 `eglGetProcAddress` 动态取扩展入口 | `glVertexAttribDivisorANGLE`、`glDrawElementsInstancedANGLE` 等；2D 纹理 | 托管 `Gles` 缺 instancing 扩展入口点与常量；仍缺纹理 API | [ ] |
| `multiview` | `GL_OVR_multiview2` 分层渲染（2 视图） | ES3 上下文 | 纹理数组（`GL_TEXTURE_2D_ARRAY`/`glTexImage3D`）、`glFramebufferTextureMultiviewOVR`、FBO、VAO/VBO、depth | 托管 `Gles` 缺 ES3 纹理数组/FBO/VAO 完整覆盖；缺 multiview 扩展入口点与扩展检测封装 | [ ] |
| `post_sub_buffer` | `eglPostSubBufferNV` 局部更新（替代 SwapBuffers） | `EGL_NV_post_sub_buffer` 扩展 + `eglPostSubBufferNV` | 基础 GLES2 draw（无特殊 GL 扩展） | Native Bridge 未暴露 `eglPostSubBufferNV`；托管层需支持 EGL 扩展入口加载与 API surface | [ ] |
| `multi_window` | 多窗口 + 多 EGLSurface；同一 EGLContext 轮流绑定/Swap | **多 OSWindow** + `eglCreateWindowSurface` 多次；每帧 `eglMakeCurrent(display, surface, …)` 切换；每窗口 `eglSwapBuffers` | 基础 GLES2 draw + 矩阵/变换（CPU 侧） | Native Bridge 不暴露 `EGLDisplay/EGLConfig/EGLSurface` 与 `eglCreateWindowSurface/eglMakeCurrent` 原语（无法一 context 多 surface）；托管侧也缺多窗口 runner | [ ] |
| `multiple_contexts` | 多线程/多上下文共享；后台线程更新纹理并同步 | `eglCreateContext(..., shareContext, ...)`、`eglCreatePbufferSurface`、跨线程 `eglMakeCurrent`/解绑/销毁 | GLES3：`glFenceSync/glClientWaitSync`；纹理、VBO（`glBufferData/SubData`） | Native Bridge 缺 share-context + 多 context/多 surface API；托管 `Gles` 缺 GLES3 sync/纹理/VBO 完整覆盖与线程亲和约束建模 | [ ] |
| `WindowTest` | OSWindow 行为测试（尺寸/位置/事件/可见性） | 无（非 EGL 路径） | 无（非 GL 路径） | 托管侧若要“覆盖”此目录，需要等价的窗口抽象/事件系统（WinForms 可作为实现之一） | [ ] |
| `shader_translator` | ShaderLang 翻译器 CLI（GLSL/ESSL/HLSL/MSL/SPIR-V） | 无（非 EGL 路径） | `GLSLANG/ShaderLang.h` 相关 API（ANGLE 编译器） | 需要单独规划：托管 ShaderLang（P/Invoke/绑定生成）或保留 native tool；不属于 EGL/GLES 渲染路径 | [ ] |
| `gles1` | GLES1 固定管线一组样例（含纹理/光照/DrawTexture） | ES1 上下文 | GLES1 固定管线：`glEnableClientState/glVertexPointer/glMatrixMode/glLight*` 等；`GL_OES_draw_texture`（`glDrawTex*OES`/`GL_TEXTURE_CROP_RECT_OES`） | 当前托管层几乎全是 GLES2/3 子集：需新增 GLES1 binding（或明确放弃 GLES1 支持）+ 对应 sample runner | [ ] |
| `torus_lighting` | Torus 光照：ES1 固定管线、ES2 shader；另含 ES3.1 buffer storage 示例 | ES1/ES2/ES3.1 上下文选择；扩展检测 | ES1：matrix stack + lighting；ES2：shader+matrix uniform；ES3.1：`GL_EXT_buffer_storage`（`glBufferStorageEXT`/persistent map）+ sync | 需要补齐三条线：GLES1 binding、GLES2 matrix/uniform 完整覆盖、GLES3.1 + buffer_storage 扩展入口与 sync；当前差距较大 | [ ] |
| `capture_replay` | Trace 回放（重放一段 capture） | ES3 上下文 + 常规 swap/loop | Trace replay 运行时（TraceLibrary/trace 数据） | 需要集成/托管 ANGLE capture & replay 体系（trace 动态库加载、数据目录、重置/回放接口）；不只是补 GL 入口 | [ ] |
