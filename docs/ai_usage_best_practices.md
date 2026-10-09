# Development Assistant Guidelines

This document records practical guidelines for using coding assistants on angle-dotnet.

- Keep changes focused and reviewable.
- Read repository instructions before editing code.
- Preserve the native/managed layering and thread-affinity rules.
- Run generator tests and managed builds after binding changes.
- Never commit secrets, local build outputs, or proprietary assets.
- Prefer deterministic demos and pixel comparisons for rendering changes.
- Document assumptions, limitations, and validation commands in pull requests.
