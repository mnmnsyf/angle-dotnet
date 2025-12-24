using AngleBridge.Sdk.Runtime;
using Demo3_Triangle.Plugin;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace Demo3_Triangle.Offscreen
{
    internal static class Program
    {
        private static volatile int s_benchSink;

        private static int Main(string[] args)
        {
            var width = 256;
            var height = 256;
            var outPath = "demo3_csharp.ppm";
            var warmup = 200;
            var frames = 1000;
            var perf = false;
            var csvPath = (string?)null;
            var pinvokeCalls = 5_000_000;
            var glBatchCalls = 1_000_000;
            var work = 1;
            var noGpuTimer = false;

            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--out":
                        if (i + 1 < args.Length) outPath = args[++i];
                        break;
                    case "--w":
                        if (i + 1 < args.Length) width = int.Parse(args[++i], CultureInfo.InvariantCulture);
                        break;
                    case "--h":
                        if (i + 1 < args.Length) height = int.Parse(args[++i], CultureInfo.InvariantCulture);
                        break;
                    case "--perf":
                        perf = true;
                        break;
                    case "--csv":
                        if (i + 1 < args.Length) csvPath = args[++i];
                        break;
                    case "--warmup":
                        if (i + 1 < args.Length) warmup = int.Parse(args[++i], CultureInfo.InvariantCulture);
                        break;
                    case "--frames":
                        if (i + 1 < args.Length) frames = int.Parse(args[++i], CultureInfo.InvariantCulture);
                        break;
                    case "--pinvoke":
                        if (i + 1 < args.Length) pinvokeCalls = int.Parse(args[++i], CultureInfo.InvariantCulture);
                        break;
                    case "--glbatch":
                        if (i + 1 < args.Length) glBatchCalls = int.Parse(args[++i], CultureInfo.InvariantCulture);
                        break;
                    case "--work":
                        if (i + 1 < args.Length) work = int.Parse(args[++i], CultureInfo.InvariantCulture);
                        break;
                    case "--no-gpu-timer":
                        noGpuTimer = true;
                        break;
                }
            }

            if (width <= 0 || height <= 0)
            {
                Console.Error.WriteLine("Width/height must be positive.");
                return 1;
            }

            if (perf)
            {
                if (frames <= 0 || warmup < 0)
                {
                    Console.Error.WriteLine("Frames must be positive and warmup must be non-negative.");
                    return 1;
                }
                if (work <= 0)
                {
                    Console.Error.WriteLine("Work must be positive.");
                    return 1;
                }

                var path = csvPath ?? "demo3_csharp_perf.csv";
                var ok = RunPerf(width, height, warmup, frames, work, noGpuTimer, path, pinvokeCalls, glBatchCalls);
                return ok ? 0 : 1;
            }

            using (var device = AngleDevice.CreateOffscreen(width, height, new AngleDeviceOptions { GlesMajor = 3 }))
            {
                using (var renderer = new TriangleRenderer())
                {
                    renderer.Draw(device.Gl, width, height);
                }

                device.Gl.Finish();

                var rgba = new byte[checked(width * height * 4)];
                device.ReadPixelsRgba(0, 0, width, height, rgba, 0);

                EnsureOutputDirectory(outPath);
                WritePpm(outPath, width, height, rgba);
            }

            Console.WriteLine("Wrote: {0}", outPath);
            return 0;
        }

        private static bool RunPerf(int width, int height, int warmup, int frames, int work, bool noGpuTimer, string csvPath, int pinvokeCalls, int glBatchCalls)
        {
            using (var device = AngleDevice.CreateOffscreen(width, height, new AngleDeviceOptions { GlesMajor = 3 }))
            {
                var gl = device.Gl;

                var pinvokeNoOpNs = MeasureNoOpBaselineSubtract(device, pinvokeCalls);
                var pinvokeNoOpNNs = MeasureNoOpN(device, pinvokeCalls);
                var nativeGlCallNs = MeasureGlCallBatch(device, glBatchCalls);

                using (var renderer = new TriangleRenderer())
                {
                    var totalFrames = warmup + frames;
                    var cpuSubmitUs = new double[frames];
                    var cpuFrameUs = new double[frames];
                    var gpuUs = new double[frames];
                    for (var i = 0; i < gpuUs.Length; i++) gpuUs[i] = -1.0;

                    var useGpuTimer = gl.IsDisjointTimerQueryAvailable && !noGpuTimer;
                    var queries = useGpuTimer ? new[] { gl.CreateTimerQuery(), gl.CreateTimerQuery(), gl.CreateTimerQuery() } : Array.Empty<int>();

                    for (var i = 0; i < totalFrames; i++)
                    {
                        var frameStart = Stopwatch.GetTimestamp();

                        if (useGpuTimer && i >= 3)
                        {
                            TryReadGpuQuery(gl, queries, warmup, i - 3, gpuUs, force: true);
                        }

                        var submitStart = Stopwatch.GetTimestamp();
                        if (useGpuTimer) gl.BeginTimerQuery(queries[i % 3]);

                        for (var j = 0; j < work; j++)
                        {
                            renderer.Draw(gl, width, height);
                        }

                        if (useGpuTimer) gl.EndTimerQuery();
                        var submitEnd = Stopwatch.GetTimestamp();

                        if (useGpuTimer && i >= 2)
                        {
                            TryReadGpuQuery(gl, queries, warmup, i - 2, gpuUs, force: false);
                        }

                        var frameEnd = Stopwatch.GetTimestamp();

                        if (i >= warmup)
                        {
                            var idx = i - warmup;
                            cpuSubmitUs[idx] = TicksToMicroseconds(submitEnd - submitStart);
                            cpuFrameUs[idx] = TicksToMicroseconds(frameEnd - frameStart);
                        }
                    }

                    if (useGpuTimer)
                    {
                        gl.Finish();
                        TryReadGpuQuery(gl, queries, warmup, totalFrames - 3, gpuUs, force: true);
                        TryReadGpuQuery(gl, queries, warmup, totalFrames - 2, gpuUs, force: true);
                        TryReadGpuQuery(gl, queries, warmup, totalFrames - 1, gpuUs, force: true);

                        for (var i = 0; i < queries.Length; i++) gl.DeleteTimerQuery(queries[i]);
                    }

                    EnsureOutputDirectory(csvPath);
                    WritePerfCsv(
                        csvPath,
                        cpuSubmitUs,
                        cpuFrameUs,
                        gpuUs,
                        useGpuTimer,
                        pinvokeNoOpNs,
                        pinvokeNoOpNNs,
                        nativeGlCallNs);
                }
            }

            Console.WriteLine("Wrote: {0}", csvPath);
            return true;
        }

        private static void TryReadGpuQuery(AngleGl gl, int[] queries, int warmup, int frameIndex, double[] gpuUs, bool force)
        {
            if (frameIndex < warmup) return;
            var outIndex = frameIndex - warmup;
            if (outIndex < 0 || outIndex >= gpuUs.Length) return;
            if (gpuUs[outIndex] >= 0.0) return;

            var queryId = queries[frameIndex % 3];
            if (!gl.TryGetTimerQueryResultAvailable(queryId, out var available)) return;
            if (!available && !force) return;

            if (gl.TryGetGpuDisjoint(out var disjoint) && disjoint)
            {
                gpuUs[outIndex] = -1.0;
                return;
            }

            if (gl.TryGetTimerQueryResult(queryId, out var ns))
            {
                gpuUs[outIndex] = ns / 1000.0;
            }
        }

        private static double MeasureNoOpBaselineSubtract(AngleDevice device, int iterations)
        {
            if (iterations <= 0) return double.NaN;
            for (var i = 0; i < 1000; i++) device.NativeNoOp();

            long? bestDelta = null;
            for (var rep = 0; rep < 7; rep++)
            {
                var baseline = MeasureLoopBaselineTicks(iterations);
                var withCall = MeasureLoopWithNoOpTicks(device, iterations);
                var delta = withCall - baseline;
                if (delta <= 0) continue;
                if (bestDelta == null || delta < bestDelta.Value) bestDelta = delta;
            }

            if (bestDelta == null) return double.NaN;
            return TicksToNanoseconds(bestDelta.Value) / iterations;
        }

        private static long MeasureLoopBaselineTicks(int iterations)
        {
            var t0 = Stopwatch.GetTimestamp();
            for (var i = 0; i < iterations; i++)
            {
                s_benchSink ^= i;
            }
            return Stopwatch.GetTimestamp() - t0;
        }

        private static long MeasureLoopWithNoOpTicks(AngleDevice device, int iterations)
        {
            var t0 = Stopwatch.GetTimestamp();
            for (var i = 0; i < iterations; i++)
            {
                s_benchSink ^= i;
                device.NativeNoOp();
            }
            return Stopwatch.GetTimestamp() - t0;
        }

        private static double MeasureNoOpN(AngleDevice device, int iterations)
        {
            if (iterations <= 0) return double.NaN;
            device.NativeNoOpN(1);

            var t0 = Stopwatch.GetTimestamp();
            device.NativeNoOpN(iterations);
            var t1 = Stopwatch.GetTimestamp();

            return TicksToNanoseconds(t1 - t0) / iterations;
        }

        private static double MeasureGlCallBatch(AngleDevice device, int iterations)
        {
            if (iterations <= 0) return double.NaN;
            device.NativeGlCallBatch(1);

            var t0 = Stopwatch.GetTimestamp();
            device.NativeGlCallBatch(iterations);
            var t1 = Stopwatch.GetTimestamp();

            return TicksToNanoseconds(t1 - t0) / iterations;
        }

        private static double TicksToMicroseconds(long ticks) =>
            ticks * (1_000_000.0 / Stopwatch.Frequency);

        private static double TicksToNanoseconds(long ticks) =>
            ticks * (1_000_000_000.0 / Stopwatch.Frequency);

        private static void EnsureOutputDirectory(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (string.IsNullOrWhiteSpace(dir)) return;
            Directory.CreateDirectory(dir);
        }

        private static void WritePerfCsv(
            string path,
            double[] cpuSubmitUs,
            double[] cpuFrameUs,
            double[] gpuUs,
            bool hasGpu,
            double pinvokeNoOpNs,
            double pinvokeNoOpNNs,
            double pinvokeGlCallNs)
        {
            var culture = CultureInfo.InvariantCulture;
            double sumSubmit = 0;
            double sumFrame = 0;
            double sumGpu = 0;
            var gpuCount = 0;

            for (var i = 0; i < cpuSubmitUs.Length; i++)
            {
                sumSubmit += cpuSubmitUs[i];
                sumFrame += cpuFrameUs[i];
                if (gpuUs[i] >= 0)
                {
                    sumGpu += gpuUs[i];
                    gpuCount++;
                }
            }

            var avgSubmit = cpuSubmitUs.Length > 0 ? sumSubmit / cpuSubmitUs.Length : 0.0;
            var avgFrame = cpuFrameUs.Length > 0 ? sumFrame / cpuFrameUs.Length : 0.0;
            var avgGpu = (hasGpu && gpuCount > 0) ? (sumGpu / gpuCount) : double.NaN;

            using (var writer = new StreamWriter(path, false, new UTF8Encoding(false)))
            {
                writer.WriteLine("frame,cpu_submit_us,cpu_frame_us,gpu_us");
                for (var i = 0; i < cpuSubmitUs.Length; i++)
                {
                    writer.Write(i.ToString(culture));
                    writer.Write(",");
                    writer.Write(cpuSubmitUs[i].ToString("F3", culture));
                    writer.Write(",");
                    writer.Write(cpuFrameUs[i].ToString("F3", culture));
                    writer.Write(",");
                    if (gpuUs[i] >= 0)
                        writer.Write(gpuUs[i].ToString("F3", culture));
                    else
                        writer.Write("N/A");
                    writer.WriteLine();
                }

                writer.Write("avg,");
                writer.Write(avgSubmit.ToString("F3", culture));
                writer.Write(",");
                writer.Write(avgFrame.ToString("F3", culture));
                writer.Write(",");
                writer.Write(hasGpu && !double.IsNaN(avgGpu) ? avgGpu.ToString("F3", culture) : "N/A");
                writer.WriteLine();

                writer.Write("pinvoke_noop_ns_per_call,");
                writer.Write(double.IsNaN(pinvokeNoOpNs) ? "N/A" : pinvokeNoOpNs.ToString("F3", culture));
                writer.WriteLine(",," );

                writer.Write("pinvoke_noopn_ns_per_iter,");
                writer.Write(double.IsNaN(pinvokeNoOpNNs) ? "N/A" : pinvokeNoOpNNs.ToString("F3", culture));
                writer.WriteLine(",," );

                writer.Write("native_glcall_ns_per_iter,");
                writer.Write(double.IsNaN(pinvokeGlCallNs) ? "N/A" : pinvokeGlCallNs.ToString("F3", culture));
                writer.WriteLine(",," );
            }
        }

        private static void WritePpm(string path, int width, int height, byte[] rgba)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var header = string.Format(CultureInfo.InvariantCulture, "P6\n{0} {1}\n255\n", width, height);
                var headerBytes = Encoding.ASCII.GetBytes(header);
                fs.Write(headerBytes, 0, headerBytes.Length);

                for (var y = height - 1; y >= 0; y--)
                {
                    var row = y * width * 4;
                    for (var x = 0; x < width; x++)
                    {
                        var i = row + x * 4;
                        fs.WriteByte(rgba[i + 0]);
                        fs.WriteByte(rgba[i + 1]);
                        fs.WriteByte(rgba[i + 2]);
                    }
                }
            }
        }
    }
}
