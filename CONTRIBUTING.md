# Contributing to angle-dotnet

Thank you for your interest in contributing to angle-dotnet.

## Before opening an issue

- Search existing issues and discussions first.
- Include the operating system, GPU, driver, .NET SDK, Visual Studio, CMake, and ANGLE versions.
- Include a minimal reproduction and the exact command that failed.
- Do not include private keys, API tokens, proprietary assets, or generated build directories.

## Development workflow

1. Fork the repository and create a focused branch.
2. Initialize the ANGLE submodule.
3. Make the smallest change that solves the problem.
4. Add or update deterministic validation where practical.
5. Run the generator tests and all managed project builds.
6. Run the native bridge and offscreen smoke tests when ANGLE runtime artifacts are available.
7. Ensure `git diff --check` is clean.
8. Open a pull request with an English description of the change.

## Code conventions

- Keep managed code compatible with C# 9 language features.
- Keep public APIs CLS-friendly.
- Keep P/Invoke, function pointers, delegates, and calling conventions internal.
- Preserve EGL context thread-affinity checks.
- Make GPU-owning resource types safely disposable.
- Keep generated files reproducible through `scripts/gen_gles30_managed.py`.
- Use Conventional Commit style where practical, for example `fix(bindings): preserve out parameters`.

## Pull requests

A pull request should explain:

- What changed and why.
- Which tests and builds were run.
- Any platform or driver limitations.
- Any follow-up work that remains.

All code, comments, documentation, issues, and pull requests should use English.
