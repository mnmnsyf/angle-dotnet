#!/usr/bin/env python3
# how to use: python scripts/compare_demo3.py --configuration Debug --width 256 --height 256
from __future__ import annotations

import argparse
import os
import subprocess
import sys
from pathlib import Path


def run(cmd: list[str], cwd: Path | None = None) -> None:
    subprocess.run(cmd, cwd=cwd, check=True)


def read_all(path: Path) -> bytes:
    return path.read_bytes()


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--configuration", "-c", choices=["Debug", "Release"], default="Debug")
    parser.add_argument("--width", "--w", type=int, default=256)
    parser.add_argument("--height", "--h", type=int, default=256)
    parser.add_argument("--outdir", default="")
    args = parser.parse_args()

    if args.width <= 0 or args.height <= 0:
        print("Width/height must be positive.", file=sys.stderr)
        return 1

    script_dir = Path(__file__).resolve().parent
    root = script_dir.parent
    outdir = Path(args.outdir) if args.outdir else script_dir / "out"
    outdir.mkdir(parents=True, exist_ok=True)

    cpp_out = outdir / "demo3_cpp.ppm"
    cs_out = outdir / "demo3_csharp.ppm"

    preset = "build-release" if args.configuration == "Release" else "build-debug"
    run(["cmake", "--build", "--preset", preset, "--target", "demo3_native"], cwd=root)

    csproj = root / "demos" / "Demo3_Triangle.Offscreen" / "Demo3_Triangle.Offscreen.csproj"
    run(["dotnet", "build", str(csproj), "-c", args.configuration], cwd=root)

    if args.configuration == "Release":
        cpp_exe = root / "build" / "vs2022-release" / "demos" / "demo3_native" / "Release" / "demo3_native.exe"
    else:
        cpp_exe = root / "build" / "vs2022-debug" / "demos" / "demo3_native" / "Debug" / "demo3_native.exe"

    if not cpp_exe.exists():
        print(f"demo3_native.exe not found at {cpp_exe}", file=sys.stderr)
        return 1

    run([str(cpp_exe), "--out", str(cpp_out), "--w", str(args.width), "--h", str(args.height)], cwd=root)

    cs_exe = root / "demos" / "Demo3_Triangle.Offscreen" / "bin" / args.configuration / "net9.0" / "Demo3_Triangle.Offscreen.exe"
    cs_dll = root / "demos" / "Demo3_Triangle.Offscreen" / "bin" / args.configuration / "net9.0" / "Demo3_Triangle.Offscreen.dll"
    if cs_exe.exists():
        run([str(cs_exe), "--out", str(cs_out), "--w", str(args.width), "--h", str(args.height)], cwd=root)
    elif cs_dll.exists():
        run(["dotnet", str(cs_dll), "--out", str(cs_out), "--w", str(args.width), "--h", str(args.height)], cwd=root)
    else:
        print(f"C# output not found at {cs_exe} or {cs_dll}", file=sys.stderr)
        return 1

    a = read_all(cpp_out)
    b = read_all(cs_out)

    if len(a) != len(b):
        print(f"FAIL: size mismatch ({len(a)} vs {len(b)})")
        return 1

    for i, (ba, bb) in enumerate(zip(a, b)):
        if ba != bb:
            print(f"FAIL: first mismatch at offset {i} (cpp={ba} csharp={bb})")
            return 1

    pixels = args.width * args.height
    print(f"PASS: All {pixels} pixels are exactly the same!")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

