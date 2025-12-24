#!/usr/bin/env python3
"""
Builds C++ and C# projects (Release), runs benchmarks, and generates a comparison CSV report.
Usage: python scripts/benchmark.py [--work 100] [--frames 1000] [--profile]
"""
import argparse
import csv
import math
import os
import shutil
import subprocess
import sys
from pathlib import Path
from typing import Dict, List, Optional, Tuple

def run_cmd(cmd, cwd=None, env=None):
    print(f"Running: {' '.join(cmd)}")
    subprocess.check_call(cmd, cwd=cwd, env=env)

def build_native(root):
    print("Building Native (Release)...")
    preset = "build-release"
    try:
        run_cmd(["cmake", "--build", "--preset", preset, "--target", "demo3_native"], cwd=root)
    except subprocess.CalledProcessError:
        print("Build failed. Trying to configure first...")
        run_cmd(["cmake", "--preset", "vs2022-release"], cwd=root) 
        run_cmd(["cmake", "--build", "--preset", preset, "--target", "demo3_native"], cwd=root)

def build_managed(root):
    print("Building Managed (Release)...")
    project = root / "demos" / "Demo3_Triangle.Offscreen" / "Demo3_Triangle.Offscreen.csproj"
    run_cmd(["dotnet", "build", str(project), "-c", "Release"], cwd=root)

# --- Statistics & CSV Parsing Helpers (integrated from compare_perf_csv.py) ---

def _try_float(s: str) -> Optional[float]:
    s = s.strip()
    if not s or s.upper() == "N/A":
        return None
    try:
        return float(s)
    except Exception:
        return None

def _percentile(sorted_vals: List[float], p: float) -> float:
    if not sorted_vals:
        return float("nan")
    if p <= 0.0:
        return sorted_vals[0]
    if p >= 1.0:
        return sorted_vals[-1]
    i = int(math.floor(p * (len(sorted_vals) - 1)))
    return sorted_vals[i]

def _stats(vals: List[float]) -> Dict[str, float]:
    if not vals:
        return {"count": 0.0}
    s = sorted(vals)
    n = len(s)
    mean = sum(s) / n
    return {
        "count": float(n),
        "mean": mean,
        "p50": _percentile(s, 0.50),
        "p95": _percentile(s, 0.95),
        "p99": _percentile(s, 0.99),
        "min": s[0],
        "max": s[-1],
    }

PerfData = Tuple[List[float], List[float], List[Optional[float]], Dict[str, Optional[float]]]

def load_perf_csv(path: Path) -> PerfData:
    cpu_submit_us: List[float] = []
    cpu_frame_us: List[float] = []
    gpu_us: List[Optional[float]] = []
    extra: Dict[str, Optional[float]] = {}

    if not path.exists():
        return ([], [], [], {})

    with path.open("r", newline="", encoding="utf-8-sig") as f:
        reader = csv.reader(f)
        header = next(reader, None)
        if not header or len(header) < 4:
            # Maybe return empty or raise? Let's just return empty for robustness
            return ([], [], [], {})

        for row in reader:
            if not row: continue
            key = row[0].strip()
            if not key: continue

            if key.isdigit():
                if len(row) < 4: continue
                cpu_submit_us.append(float(row[1]))
                cpu_frame_us.append(float(row[2]))
                gpu_us.append(_try_float(row[3]))
                continue

            if len(row) >= 2:
                extra[key] = _try_float(row[1])
            else:
                extra[key] = None

    return (cpu_submit_us, cpu_frame_us, gpu_us, extra)

def _fmt(v: Optional[float], unit: str = "") -> str:
    if v is None or math.isnan(v):
        return "N/A"
    return f"{v:.3f}{unit}"

def _ratio(a: Optional[float], b: Optional[float]) -> Optional[float]:
    if a is None or b is None or b == 0:
        return None
    return a / b

def _delta_pct(a: Optional[float], b: Optional[float]) -> Optional[float]:
    r = _ratio(a, b)
    if r is None:
        return None
    return (r - 1.0) * 100.0

# --- Comparison Reporting ---

def print_comparison_and_save(native_path: Path, managed_path: Path, out_summary_csv: Path = None):
    a_submit, a_frame, a_gpu_opt, a_extra = load_perf_csv(native_path)
    b_submit, b_frame, b_gpu_opt, b_extra = load_perf_csv(managed_path)

    if not a_submit or not b_submit:
        print("Error: Could not load data from one or both CSVs.")
        return

    a_gpu = [v for v in a_gpu_opt if v is not None]
    b_gpu = [v for v in b_gpu_opt if v is not None]

    a_submit_s = _stats(a_submit)
    a_frame_s = _stats(a_frame)
    a_gpu_s = _stats(a_gpu)

    b_submit_s = _stats(b_submit)
    b_frame_s = _stats(b_frame)
    b_gpu_s = _stats(b_gpu)

    rows = []

    def add_metric(name: str, a_val: Optional[float], b_val: Optional[float], unit: str = "") -> None:
        rows.append((
            name,
            a_val,
            b_val,
            _ratio(b_val, a_val),
            _delta_pct(b_val, a_val),
            unit,
        ))

    add_metric("samples", float(len(a_submit)), float(len(b_submit)), "")
    add_metric("avg_cpu_submit_us", a_submit_s.get("mean"), b_submit_s.get("mean"), "us")
    add_metric("p95_cpu_submit_us", a_submit_s.get("p95"), b_submit_s.get("p95"), "us")
    add_metric("avg_cpu_frame_us", a_frame_s.get("mean"), b_frame_s.get("mean"), "us")
    add_metric("p95_cpu_frame_us", a_frame_s.get("p95"), b_frame_s.get("p95"), "us")
    add_metric("avg_gpu_us", a_gpu_s.get("mean"), b_gpu_s.get("mean"), "us")
    add_metric("p95_gpu_us", a_gpu_s.get("p95"), b_gpu_s.get("p95"), "us")

    extra_keys = sorted(set(a_extra.keys()) | set(b_extra.keys()))
    for k in extra_keys:
        if k == "avg": continue
        add_metric(k, a_extra.get(k), b_extra.get(k), "")

    print("\n" + "="*80)
    print(f"{'Metric':<35} | {'C++ (A)':<12} | {'C# (B)':<12} | {'B/A':<6} | {'Delta':<8}")
    print("-" * 80)
    
    for name, a_val, b_val, ratio, dp, unit in rows:
        a_s = _fmt(a_val, unit)
        b_s = _fmt(b_val, unit)
        r_s = _fmt(ratio, "") if ratio is not None else "-"
        dp_s = f"{dp:+.1f}%" if dp is not None else "-"
        print(f"{name:<35} | {a_s:<12} | {b_s:<12} | {r_s:<6} | {dp_s:<8}")
    print("="*80 + "\n")

    if out_summary_csv:
        out_summary_csv.parent.mkdir(parents=True, exist_ok=True)
        with out_summary_csv.open("w", newline="", encoding="utf-8") as f:
            w = csv.writer(f)
            w.writerow(["metric", "C++", "C#", "Ratio", "DeltaPct"])
            for name, a_val, b_val, ratio, dp, unit in rows:
                w.writerow([
                    name,
                    a_val if a_val is not None else "N/A",
                    b_val if b_val is not None else "N/A",
                    ratio if ratio is not None else "N/A",
                    dp if dp is not None else "N/A"
                ])
        print(f"Summary saved to: {out_summary_csv}")

# --- Main ---

def main():
    parser = argparse.ArgumentParser(description="Run ANGLE .NET benchmarks")
    parser.add_argument("--skip-build", action="store_true", help="Skip build step")
    parser.add_argument("--width", type=int, default=256)
    parser.add_argument("--height", type=int, default=256)
    parser.add_argument("--frames", type=int, default=1000)
    parser.add_argument("--warmup", type=int, default=200)
    parser.add_argument("--work", type=int, default=10, help="Draw calls per frame")
    parser.add_argument("--out-dir", default="test/benchmark", help="Output directory for CSVs")
    parser.add_argument("--profile", action="store_true", help="Profile managed code with dotnet-trace and speedscope format")
    args = parser.parse_args()

    root = Path(__file__).resolve().parent.parent
    out_dir = Path(args.out_dir)
    if not out_dir.is_absolute():
        out_dir = root / out_dir
    out_dir.mkdir(exist_ok=True, parents=True)

    if not args.skip_build:
        # Build Native (required for C# dependency)
        build_native(root)
        build_managed(root)

    native_exe = root / "build" / "vs2022-release" / "demos" / "demo3_native" / "Release" / "demo3_native.exe"
    managed_dll = root / "demos" / "Demo3_Triangle.Offscreen" / "bin" / "Release" / "net9.0" / "Demo3_Triangle.Offscreen.dll"
    
    if not native_exe.exists():
        native_exe_alt = root / "build" / "demos" / "demo3_native" / "Release" / "demo3_native.exe"
        if native_exe_alt.exists():
            native_exe = native_exe_alt
        else:
            print(f"Error: Native executable not found at {native_exe}")
            return 1

    if args.profile:
        # Check if dotnet-trace is installed
        if shutil.which("dotnet-trace") is None:
            print("\nError: 'dotnet-trace' not found.")
            print("Please install it using: dotnet tool install --global dotnet-trace")
            return 1
            
        trace_path = out_dir / "trace.speedscope.json"
        print(f"\nProfiling Managed Code -> {trace_path}")
        print("Note: This may take longer due to tracing overhead.")
        
        # Build command for dotnet-trace
        profile_cmd = [
            "dotnet-trace", "collect",
            "--name", "dotnet", # trace the dotnet process we are about to launch? No, use 'collect -- <cmd>' syntax
            "--format", "speedscope",
            "--output", str(trace_path),
            "--",
            "dotnet", str(managed_dll),
            "--perf",
            "--csv", str(out_dir / "managed_profiled.csv"), # separate csv for profiled run
            "--w", str(args.width),
            "--h", str(args.height),
            "--frames", str(args.frames),
            "--warmup", str(args.warmup),
            "--work", str(args.work)
        ]
        
        # Note: 'dotnet-trace collect -- dotnet ...' requires dotnet-trace 5.0+
        # If 'dotnet-trace' is just 'dotnet-trace', subprocess works fine.
        # However, the syntax is: dotnet-trace collect --output <file> -- dotnet <app> <args>
        
        real_cmd = [
            "dotnet-trace", "collect",
            "--format", "speedscope",
            "--output", str(trace_path),
            "--",
            "dotnet", str(managed_dll),
            "--perf",
            "--csv", str(out_dir / "managed_profiled.csv"),
            "--w", str(args.width),
            "--h", str(args.height),
            "--frames", str(args.frames),
            "--warmup", str(args.warmup),
            "--work", str(args.work)
        ]
        
        run_cmd(real_cmd, cwd=root)
        
        print("\nProfiling Complete!")
        print(f"Trace file: {trace_path}")
        print("View the flamegraph by uploading this file to: https://www.speedscope.app/")
        return 0

    native_csv = out_dir / "native.csv"
    managed_csv = out_dir / "managed.csv"
    summary_csv = out_dir / "summary.csv"

    print(f"Running Native Benchmark -> {native_csv}")
    run_cmd([
        str(native_exe),
        "--perf",
        "--csv", str(native_csv),
        "--w", str(args.width),
        "--h", str(args.height),
        "--frames", str(args.frames),
        "--warmup", str(args.warmup),
        "--work", str(args.work)
    ], cwd=root)

    print(f"Running Managed Benchmark -> {managed_csv}")
    run_cmd([
        "dotnet", str(managed_dll),
        "--perf",
        "--csv", str(managed_csv),
        "--w", str(args.width),
        "--h", str(args.height),
        "--frames", str(args.frames),
        "--warmup", str(args.warmup),
        "--work", str(args.work)
    ], cwd=root)

    print_comparison_and_save(native_csv, managed_csv, summary_csv)

if __name__ == "__main__":
    sys.exit(main())
