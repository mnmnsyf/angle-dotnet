# angle-dotnet

A .NET/OpenGL ES SDK built on [ANGLE](https://chromium.googlesource.com/angle/angle/). The project provides a native EGL bridge, managed GLES bindings, a CLS-friendly SDK facade, optional utilities, and a collection of deterministic rendering demos.

## What this project provides

- Offscreen rendering with pixel readback.
- Window-surface rendering with swap/present support.
- ANGLE backend and device selection on Windows.
- Dynamic GLES entry-point loading through `eglGetProcAddress`.
- Generated GLES 3.0 managed bindings with reproducible output.
- CLS-friendly, context-bound SDK types and disposable GPU resources.
- WinForms sample hosting and collectible AssemblyLoadContext hot reload.
- Texture, shader, framebuffer, sampler, query, sync, and transform-feedback helpers.
- C++/C# offscreen comparison using PPM output.
- Rendering demos covering shaders, textures, mipmaps, cubemaps, stencil/depth, instancing, multiview, multiple render targets, post-sub-buffer updates, and more.

## Repository layout

| Path | Description |
| --- | --- |
| `native/AngleBridge` | Native ANGLE/EGL bridge DLL. |
| `cs/AngleBridge.Managed` | Internal P/Invoke and dynamically loaded GLES bindings. |
| `cs/AngleBridge.Sdk` | Public CLS-friendly .NET SDK facade. |
| `cs/AngleBridge.Utilities` | Optional sample runner and texture utilities. |
| `cs/AngleBridge.Utilities/WinForms` | Windows Forms window and hot-reload host. |
| `demos/` | Native, offscreen, WinForms, and plugin-based demos. |
| `scripts/` | Binding generation, image comparison, and benchmark scripts. |
| `docs/` | Architecture, profiling, and sample coverage documentation. |
| `external/angle` | ANGLE Git submodule. |

## Requirements

- Windows 10/11 x64 for the native bridge and WinForms samples.
- .NET SDK 9.0.318 or a compatible .NET 9 SDK.
- CMake 3.20 or newer.
- Visual Studio 2022 with the C++ desktop workload.
- Python 3.9 or newer for generator tests and validation scripts.
- A separately built ANGLE runtime (`libEGL.dll` and `libGLESv2.dll`) for native execution.

The managed projects target `net9.0`. The WinForms host targets `net9.0-windows`.

## Clone and initialize dependencies

```powershell
git clone https://github.com/<your-account>/angle-dotnet.git
cd angle-dotnet
git submodule update --init --recursive
```

The ANGLE submodule is intentionally kept separate because ANGLE is a large upstream project with its own build system and dependencies.

## Build ANGLE

Build ANGLE using ANGLE's documented GN/Ninja workflow. A typical Windows build is:

```powershell
cd external/angle
python scripts/bootstrap.py
python scripts/gn.py gen out/Release --args="is_debug=false angle_assert_always_on=false"
autoninja -C out/Release libEGL libGLESv2
cd ../..
```

The exact ANGLE build arguments depend on the graphics backends and SDKs installed on the machine.

## Build the native bridge

The repository contains Visual Studio CMake presets:

```powershell
cmake --preset vs2022-debug
cmake --build --preset build-debug --target AngleBridge

cmake --preset vs2022-release
cmake --build --preset build-release --target AngleBridge
```

The presets expect ANGLE libraries under `external/angle/out/Debug` or `external/angle/out/Release`.

## Build all managed projects

```powershell
$projects = Get-ChildItem cs,demos -Recurse -Filter *.csproj -File |
  Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }

foreach ($project in $projects) {
  dotnet build $project.FullName -c Debug --no-restore
}
```

Or build the primary SDK projects directly:

```powershell
dotnet build cs/AngleBridge.Managed/AngleBridge.Managed.csproj -c Release
dotnet build cs/AngleBridge.Sdk/AngleBridge.Sdk.csproj -c Release
dotnet build cs/AngleBridge.Utilities/AngleBridge.Utilities.csproj -c Release
```

## Quick start: offscreen rendering

```csharp
using AngleBridge.Sdk.Runtime;

using (var device = AngleDevice.CreateOffscreen(
    256,
    256,
    new AngleDeviceOptions { GlesMajor = 3 }))
{
    var gl = device.Gl;
    gl.Draw.Finish();

    var rgba = new byte[256 * 256 * 4];
    device.ReadPixelsRgba(0, 0, 256, 256, rgba, 0);
}
```

`AngleDevice` and its associated `AngleGl` instance are thread-affine. All operations must execute on the thread that created the EGL context.

## Run the demos

Run the WinForms host with a selected demo:

```powershell
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- --demo=5 --show
```

Select an ANGLE backend and disable vsync:

```powershell
dotnet run -c Debug --project demos/DemoHost.WinForms/DemoHost.WinForms.csproj -- `
  --demo=5 --show --use-angle=d3d11 --swap-interval=0
```

Run the offscreen demo:

```powershell
dotnet run -c Debug --project demos/Demo3_Triangle.Offscreen/Demo3_Triangle.Offscreen.csproj -- `
  --out $env:TEMP\angle-dotnet-demo3.ppm --w 256 --h 256
```

Compare two PPM/PGM/PNG images:

```powershell
python scripts/compare_PPM.py image-a.ppm image-b.ppm
```

## Generate GLES bindings

The generator reads Khronos `gl.xml`, reuses existing hand-maintained delegate signatures, and emits the generated GLES 3.0 files:

```powershell
python scripts/gen_gles30_managed.py
python -m unittest discover -s scripts -p 'test_*.py' -v
```

The generator preserves existing `ref`, `out`, and `in` modifiers and is idempotent for both the default and custom output directories.

## Validation

The project uses deterministic validation where possible:

```powershell
python -m unittest discover -s scripts -p 'test_*.py' -v
cmake --build --preset build-debug --target AngleBridge
```

The offscreen C# and C++ demos can emit matching PPM images for byte-level comparison. See `docs/samples_matrix.md` for the current demo and ANGLE sample coverage.

## Profiling

ETW/WPA/PerfView guidance is available in [`docs/profiling_etw.md`](docs/profiling_etw.md).

## Current limitations

- The native bridge currently targets Windows and does not expose the full EGL object model.
- Multiple EGL surfaces, shared contexts, and arbitrary `eglMakeCurrent` switching are not yet modeled by the public API.
- The WinForms window backend is Windows-specific.
- ANGLE must be built separately; this repository does not vendor ANGLE build outputs.
- The generated bindings intentionally keep general pointer parameters as `IntPtr`; callers are responsible for native memory lifetime and layout.
- Some utility APIs remain available for compatibility and are marked obsolete in favor of the newer grouped SDK APIs.

## Contributing

See [`CONTRIBUTING.md`](CONTRIBUTING.md). All public documentation, issues, and pull requests should use English.

## License

This project is licensed under the MIT License. See [`LICENSE`](LICENSE).

ANGLE and its third-party dependencies remain subject to their respective upstream licenses. See the ANGLE submodule and its third-party license files for details.
