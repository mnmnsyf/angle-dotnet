using AngleBridge.Sdk.HotReload;
using AngleBridge.Sdk.Interop;
using AngleBridge.Sdk.Runtime;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Demo7_TriFanMicrobench.Plugin
{
    public sealed class TriFanMicrobenchRenderer : IRenderer, IRendererWithHostContext
    {
        private const int Slices = 8;
        private const int Squares = 289;
        private const float HalfDim = 0.0625f; // kept consistent with the ANGLE tri_fan_microbench sample
        private const float XMin = -1.0f;
        private const float XMax = 1.0f;
        private const float YMin = -1.0f;

        private AngleGl? _gl;
        private AngleBuffer[]? _fanBuffers;
        private AngleBuffer[]? _triBuffers;
        private AngleQuery? _fanQuery;
        private AngleQuery? _triQuery;
        private ShaderProgramReloader? _programReloader;
        private Exception? _lastReloadError;

        private int _positionLoc;
        private bool _initialized;
        private int _numFanVerts;
        private int _numTriVerts;
        private bool _gpuTimerAvailable;
        private bool _gpuDisjoint;

        private readonly Stopwatch _fanCpuTimer = new Stopwatch();
        private readonly Stopwatch _triCpuTimer = new Stopwatch();
        private double _fanCpuTotalMs;
        private double _triCpuTotalMs;
        private ulong _fanGpuTotalNs;
        private ulong _triGpuTotalNs;
        private long _frameCount;

        public void Draw(AngleGl gl, int width, int height)
        {
            if (gl == null) throw new ArgumentNullException(nameof(gl));
            if (_gl == null)
                _gl = gl;
            else if (!ReferenceEquals(_gl, gl))
                throw new InvalidOperationException("Renderer cannot be shared across different AngleGl instances.");

            if (!_initialized)
                Initialize(gl);

            var programReloader = _programReloader ?? throw new InvalidOperationException("Program reloader not initialized.");
            if (programReloader.TryReload(gl))
                UpdateProgramBindings(gl, programReloader.Current);

            var reloadError = programReloader.LastError;
            if (reloadError != null && !ReferenceEquals(reloadError, _lastReloadError))
            {
                _lastReloadError = reloadError;
                Console.Error.WriteLine(reloadError);
            }

            gl.State.Viewport(0, 0, width, height);
            gl.State.Clear(ClearBufferMask.ColorBufferBit);
            gl.Programs.UseProgram(programReloader.Current);
            gl.VertexArrays.EnableVertexAttribArray(_positionLoc);

            RunFanPass(gl);

            // Clear the color to avoid driver-side cache effects from color coverage, matching the ANGLE sample
            gl.State.Clear(ClearBufferMask.ColorBufferBit);

            RunTriPass(gl);

            _frameCount++;
        }

        public void Dispose()
        {
            if (_fanBuffers != null)
            {
                foreach (var buf in _fanBuffers)
                    buf?.Dispose();
            }

            if (_triBuffers != null)
            {
                foreach (var buf in _triBuffers)
                    buf?.Dispose();
            }

            _fanQuery?.Dispose();
            _triQuery?.Dispose();
            _programReloader?.Dispose();

            if (_frameCount > 0)
                PrintSummary();

            _fanBuffers = null;
            _triBuffers = null;
            _fanQuery = null;
            _triQuery = null;
            _initialized = false;
            _gl = null;
            _programReloader = null;
            _lastReloadError = null;
        }

        private void Initialize(AngleGl gl)
        {
            EnsureProgramReloader();
            var reloader = _programReloader ?? throw new InvalidOperationException("Program reloader not initialized.");
            reloader.Load(gl);
            UpdateProgramBindings(gl, reloader.Current);

            CreateBuffers(gl);

            _gpuTimerAvailable = gl.Queries.IsDisjointTimerQueryAvailable;
            if (_gpuTimerAvailable)
            {
                _fanQuery = gl.Queries.CreateQueryObject();
                _triQuery = gl.Queries.CreateQueryObject();
            }

            gl.State.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            _initialized = true;
        }

        private void CreateBuffers(AngleGl gl)
        {
            // TRIANGLE_FAN vertices (center + 8 slices + closing vertex)
            float[] fanVertices =
            {
                0.0f,      0.0f,      0.0f,
                -HalfDim, -HalfDim,  0.0f,
                -HalfDim,  0.0f,     0.0f,
                -HalfDim,  HalfDim,  0.0f,
                0.0f,      HalfDim,  0.0f,
                HalfDim,   HalfDim,  0.0f,
                HalfDim,   0.0f,     0.0f,
                HalfDim,  -HalfDim,  0.0f,
                0.0f,     -HalfDim,  0.0f,
                -HalfDim, -HalfDim,  0.0f,
            };

            // TRIANGLES vertices: expand the fan into slices*3 triangles
            var numFanVertices = Slices + 2;
            var fanFloats = numFanVertices * 3;
            _numFanVerts = numFanVertices;

            var numTriVertices = Slices * 3;
            _numTriVerts = numTriVertices;
            var triFloats = numTriVertices * 3;
            var triVertices = new float[triFloats];
            var triPtr = 0;
            for (var i = 0; i < Slices; i++)
            {
                // Each slice: center + fan[i+1] + fan[i+2]
                Array.Copy(fanVertices, 0, triVertices, triPtr, 3);
                triPtr += 3;
                Array.Copy(fanVertices, (i + 1) * 3, triVertices, triPtr, 3);
                triPtr += 3;
                Array.Copy(fanVertices, (i + 2) * 3, triVertices, triPtr, 3);
                triPtr += 3;
            }

            _fanBuffers = new AngleBuffer[Squares];
            _triBuffers = new AngleBuffer[Squares];

            var xOffset = XMin;
            var yOffset = YMin;
            for (var i = 0; i < Squares; i++)
            {
                var fanTemp = new float[fanFloats];
                for (var v = 0; v < numFanVertices; v++)
                {
                    fanTemp[v * 3] = fanVertices[v * 3] + xOffset;
                    fanTemp[v * 3 + 1] = fanVertices[v * 3 + 1] + yOffset;
                    fanTemp[v * 3 + 2] = 0.0f;
                }

                var triTemp = new float[triFloats];
                for (var v = 0; v < numTriVertices; v++)
                {
                    triTemp[v * 3] = triVertices[v * 3] + xOffset;
                    triTemp[v * 3 + 1] = triVertices[v * 3 + 1] + yOffset;
                    triTemp[v * 3 + 2] = 0.0f;
                }

                var fanBuf = gl.Buffers.CreateBufferObject();
                gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, fanBuf);
                gl.Buffers.BufferData(BufferTarget.ArrayBuffer, fanTemp, BufferUsage.StaticDraw);
                _fanBuffers[i] = fanBuf;

                var triBuf = gl.Buffers.CreateBufferObject();
                gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, triBuf);
                gl.Buffers.BufferData(BufferTarget.ArrayBuffer, triTemp, BufferUsage.StaticDraw);
                _triBuffers[i] = triBuf;

                xOffset += 2 * HalfDim;
                if (xOffset > XMax)
                {
                    xOffset = XMin;
                    yOffset += 2 * HalfDim;
                }
            }
        }

        private void RunFanPass(AngleGl gl)
        {
            if (_fanBuffers == null) throw new InvalidOperationException("Buffers not initialized.");

            var fanQuery = _fanQuery;
            if (_gpuTimerAvailable && !_gpuDisjoint && fanQuery != null)
                gl.Queries.BeginQuery(QueryTarget.TimeElapsed, fanQuery);

            _fanCpuTimer.Restart();
            for (var i = 0; i < _fanBuffers.Length; i++)
            {
                gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _fanBuffers[i]);
                gl.VertexArrays.VertexAttribPointer(_positionLoc, 3, VertexAttribType.Float, false, 0, 0);
                gl.Draw.DrawArrays(PrimitiveType.TriangleFan, 0, _numFanVerts);
            }
            _fanCpuTimer.Stop();
            _fanCpuTotalMs += _fanCpuTimer.Elapsed.TotalMilliseconds;

            if (_gpuTimerAvailable && !_gpuDisjoint && fanQuery != null)
                AccumulateGpuTime(gl, fanQuery, ref _fanGpuTotalNs);
        }

        private void RunTriPass(AngleGl gl)
        {
            if (_triBuffers == null) throw new InvalidOperationException("Buffers not initialized.");

            var triQuery = _triQuery;
            if (_gpuTimerAvailable && !_gpuDisjoint && triQuery != null)
                gl.Queries.BeginQuery(QueryTarget.TimeElapsed, triQuery);

            _triCpuTimer.Restart();
            for (var i = 0; i < _triBuffers.Length; i++)
            {
                gl.Buffers.BindBuffer(BufferTarget.ArrayBuffer, _triBuffers[i]);
                gl.VertexArrays.VertexAttribPointer(_positionLoc, 3, VertexAttribType.Float, false, 0, 0);
                gl.Draw.DrawArrays(PrimitiveType.Triangles, 0, _numTriVerts);
            }
            _triCpuTimer.Stop();
            _triCpuTotalMs += _triCpuTimer.Elapsed.TotalMilliseconds;

            if (_gpuTimerAvailable && !_gpuDisjoint && triQuery != null)
                AccumulateGpuTime(gl, triQuery, ref _triGpuTotalNs);
        }

        private void AccumulateGpuTime(AngleGl gl, AngleQuery query, ref ulong totalNs)
        {
            gl.Queries.EndQuery(QueryTarget.TimeElapsed);

            // Block until the result is available; this is independent of CPU timing
            gl.Draw.Finish();

            if (gl.Queries.TryGetQueryResult(query, out var ns))
            {
                totalNs += ns;
                if (gl.Caps.TryGetGpuDisjoint(out var disjoint) && disjoint)
                    _gpuDisjoint = true;
            }
            else
            {
                _gpuDisjoint = true;
            }
        }

        private void PrintSummary()
        {
            var fanGpuMs = _fanGpuTotalNs / 1_000_000.0;
            var triGpuMs = _triGpuTotalNs / 1_000_000.0;
            var fanCpuAvg = _frameCount > 0 ? _fanCpuTotalMs / _frameCount : 0.0;
            var triCpuAvg = _frameCount > 0 ? _triCpuTotalMs / _frameCount : 0.0;
            var fanGpuAvg = _frameCount > 0 ? fanGpuMs / _frameCount : 0.0;
            var triGpuAvg = _frameCount > 0 ? triGpuMs / _frameCount : 0.0;
            var inv = CultureInfo.InvariantCulture;

            var headers = new[]
            {
                "demo",
                "frames",
                "fan_cpu_ms_total",
                "fan_cpu_ms_avg",
                "tri_cpu_ms_total",
                "tri_cpu_ms_avg",
                "fan_gpu_ms_total",
                "fan_gpu_ms_avg",
                "tri_gpu_ms_total",
                "tri_gpu_ms_avg",
                "gpu_timer_available",
                "gpu_disjoint",
            };

            var values = new[]
            {
                "tri_fan_microbench",
                _frameCount.ToString(inv),
                _fanCpuTotalMs.ToString("F3", inv),
                fanCpuAvg.ToString("F3", inv),
                _triCpuTotalMs.ToString("F3", inv),
                triCpuAvg.ToString("F3", inv),
                fanGpuMs.ToString("F3", inv),
                fanGpuAvg.ToString("F3", inv),
                triGpuMs.ToString("F3", inv),
                triGpuAvg.ToString("F3", inv),
                _gpuTimerAvailable.ToString().ToLowerInvariant(),
                _gpuDisjoint.ToString().ToLowerInvariant(),
            };

            var colWidths = headers.Select((h, i) => Math.Max(h.Length, values[i].Length) + 2).ToArray();
            string Border() => "+" + string.Join("+", colWidths.Select(w => new string('-', w))) + "+";
            string Row(string[] cells) => "|" + string.Join("|", cells.Select((c, idx) =>
            {
                var pad = colWidths[idx] - c.Length;
                return " " + c + new string(' ', pad - 1);
            })) + "|";

            Console.WriteLine(Border());
            Console.WriteLine(Row(headers));
            Console.WriteLine(Border());
            Console.WriteLine(Row(values));
            Console.WriteLine(Border());

            // Write the CSV synchronously under out/tri_fan_microbench, including the header
            var outDir = Path.Combine(Directory.GetCurrentDirectory(), "out", "tri_fan_microbench");
            Directory.CreateDirectory(outDir);
            var fileName = string.Format("tri_fan_microbench_{0:yyyyMMdd_HHmmss}.csv", DateTime.UtcNow);
            var outPath = Path.Combine(outDir, fileName);
            File.WriteAllLines(outPath, new[]
            {
                string.Join(",", headers),
                string.Join(",", values),
            });
            Console.WriteLine("Results written to: " + outPath);
        }

        private void UpdateProgramBindings(AngleGl gl, AngleProgram program)
        {
            _positionLoc = gl.Programs.GetAttribLocation(program, "vPosition");
            if (_positionLoc < 0)
                throw new InvalidOperationException("Failed to resolve attribute location.");
        }

        public void SetHostContext(RendererHostContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var roots = BuildShaderRoots(context, "Demo7_TriFanMicrobench.Plugin.csproj");
            _programReloader = new ShaderProgramReloader(
                Path.Combine("Shaders", "TriFanMicrobench.vert.glsl"),
                Path.Combine("Shaders", "TriFanMicrobench.frag.glsl"),
                roots);
        }

        private void EnsureProgramReloader()
        {
            if (_programReloader != null) return;
            var roots = new[]
            {
                AppContext.BaseDirectory,
                Directory.GetCurrentDirectory(),
            };
            _programReloader = new ShaderProgramReloader(
                Path.Combine("Shaders", "TriFanMicrobench.vert.glsl"),
                Path.Combine("Shaders", "TriFanMicrobench.frag.glsl"),
                roots);
        }

        private static string[] BuildShaderRoots(RendererHostContext context, string projectFileName)
        {
            var roots = new[]
            {
                context.PluginDirectory,
                AppContext.BaseDirectory,
                Directory.GetCurrentDirectory(),
            };

            var projectRoot = TryGetProjectRootFromPluginOutput(context.PluginDirectory, projectFileName);
            if (!string.IsNullOrWhiteSpace(projectRoot))
            {
                return new[]
                {
                    projectRoot,
                    context.PluginDirectory,
                    AppContext.BaseDirectory,
                    Directory.GetCurrentDirectory(),
                };
            }

            return roots;
        }

        private static string? TryGetProjectRootFromPluginOutput(string pluginDir, string projectFileName)
        {
            try
            {
                var di = new DirectoryInfo(pluginDir);
                for (var i = 0; i < 5 && di != null; i++)
                {
                    var candidate = Path.Combine(di.FullName, projectFileName);
                    if (File.Exists(candidate))
                        return di.FullName;
                    di = di.Parent;
                }
            }
            catch
            {
            }

            return null;
        }
    }
}
