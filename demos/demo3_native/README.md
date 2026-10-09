# demo3_native

This directory contains the `demo3_native` sample for angle-dotnet. The sample demonstrates ANGLE/EGL and OpenGL ES integration through the managed SDK.

## Build

From the repository root:

```powershell
dotnet build demos/demo3_native/demo3_native.csproj -c Debug
```

If this project is a plugin, run it through the WinForms host after building the native ANGLE bridge and ANGLE runtime libraries.

## Notes

- Runtime behavior depends on the selected ANGLE backend and installed graphics driver.
- Keep generated output and benchmark files outside source control.
- See the root README for prerequisites and the complete build workflow.
