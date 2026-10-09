# Demo8_SimpleTexture2D.Plugin

This directory contains the `Demo8_SimpleTexture2D.Plugin` sample for angle-dotnet. The sample demonstrates ANGLE/EGL and OpenGL ES integration through the managed SDK.

## Build

From the repository root:

```powershell
dotnet build demos/Demo8_SimpleTexture2D.Plugin/Demo8_SimpleTexture2D.Plugin.csproj -c Debug
```

If this project is a plugin, run it through the WinForms host after building the native ANGLE bridge and ANGLE runtime libraries.

## Notes

- Runtime behavior depends on the selected ANGLE backend and installed graphics driver.
- Keep generated output and benchmark files outside source control.
- See the root README for prerequisites and the complete build workflow.
