#include "angle_bridge.h"
#include <GLES3/gl3.h>
#include <chrono>
#include <cstdio>
#include <cstdlib>
#include <cstring>
#include <filesystem>
#include <fstream>
#include <iomanip>
#include <limits>
#include <string>
#include <vector>

namespace {

#ifndef GL_TIME_ELAPSED_EXT
#define GL_TIME_ELAPSED_EXT 0x88BF
#endif

#ifndef GL_QUERY_RESULT_EXT
#define GL_QUERY_RESULT_EXT 0x8866
#endif

#ifndef GL_QUERY_RESULT_AVAILABLE_EXT
#define GL_QUERY_RESULT_AVAILABLE_EXT 0x8867
#endif

#ifndef GL_GPU_DISJOINT_EXT
#define GL_GPU_DISJOINT_EXT 0x8FBB
#endif

typedef void (GL_APIENTRY *PFNGLGENQUERIESEXTPROC)(GLsizei n, GLuint* ids);
typedef void (GL_APIENTRY *PFNGLDELETEQUERIESEXTPROC)(GLsizei n, const GLuint* ids);
typedef void (GL_APIENTRY *PFNGLBEGINQUERYEXTPROC)(GLenum target, GLuint id);
typedef void (GL_APIENTRY *PFNGLENDQUERYEXTPROC)(GLenum target);
typedef void (GL_APIENTRY *PFNGLGETQUERYOBJECTUIVEXTPROC)(GLuint id, GLenum pname, GLuint* params);
typedef void (GL_APIENTRY *PFNGLGETQUERYOBJECTUI64VEXTPROC)(GLuint id, GLenum pname, GLuint64* params);

struct TimerQueryExt {
  PFNGLGENQUERIESEXTPROC GenQueriesEXT = nullptr;
  PFNGLDELETEQUERIESEXTPROC DeleteQueriesEXT = nullptr;
  PFNGLBEGINQUERYEXTPROC BeginQueryEXT = nullptr;
  PFNGLENDQUERYEXTPROC EndQueryEXT = nullptr;
  PFNGLGETQUERYOBJECTUIVEXTPROC GetQueryObjectuivEXT = nullptr;
  PFNGLGETQUERYOBJECTUI64VEXTPROC GetQueryObjectui64vEXT = nullptr;

  bool Available() const {
    return GenQueriesEXT && DeleteQueriesEXT && BeginQueryEXT && EndQueryEXT &&
           GetQueryObjectuivEXT && GetQueryObjectui64vEXT;
  }
};

TimerQueryExt LoadTimerQueryExt() {
  TimerQueryExt ext;
  ext.GenQueriesEXT = reinterpret_cast<PFNGLGENQUERIESEXTPROC>(AB_GetProcAddress("glGenQueriesEXT"));
  ext.DeleteQueriesEXT = reinterpret_cast<PFNGLDELETEQUERIESEXTPROC>(AB_GetProcAddress("glDeleteQueriesEXT"));
  ext.BeginQueryEXT = reinterpret_cast<PFNGLBEGINQUERYEXTPROC>(AB_GetProcAddress("glBeginQueryEXT"));
  ext.EndQueryEXT = reinterpret_cast<PFNGLENDQUERYEXTPROC>(AB_GetProcAddress("glEndQueryEXT"));
  ext.GetQueryObjectuivEXT = reinterpret_cast<PFNGLGETQUERYOBJECTUIVEXTPROC>(AB_GetProcAddress("glGetQueryObjectuivEXT"));
  ext.GetQueryObjectui64vEXT = reinterpret_cast<PFNGLGETQUERYOBJECTUI64VEXTPROC>(AB_GetProcAddress("glGetQueryObjectui64vEXT"));
  return ext;
}

double ToMicroseconds(std::chrono::steady_clock::duration d) {
  return std::chrono::duration<double, std::micro>(d).count();
}

double ToNanoseconds(std::chrono::steady_clock::duration d) {
  return std::chrono::duration<double, std::nano>(d).count();
}

GLuint CompileShader(GLenum type, const char* src) {
  GLuint shader = glCreateShader(type);
  glShaderSource(shader, 1, &src, nullptr);
  glCompileShader(shader);

  GLint ok = 0;
  glGetShaderiv(shader, GL_COMPILE_STATUS, &ok);
  if (!ok) {
    GLint len = 0;
    glGetShaderiv(shader, GL_INFO_LOG_LENGTH, &len);
    std::vector<char> log(static_cast<size_t>(len > 1 ? len : 1));
    glGetShaderInfoLog(shader, static_cast<GLsizei>(log.size()), nullptr, log.data());
    std::printf("Shader compile failed:\n%s\n", log.data());
    glDeleteShader(shader);
    return 0;
  }
  return shader;
}

GLuint LinkProgram(GLuint vs, GLuint fs) {
  GLuint program = glCreateProgram();
  glAttachShader(program, vs);
  glAttachShader(program, fs);
  glLinkProgram(program);

  GLint ok = 0;
  glGetProgramiv(program, GL_LINK_STATUS, &ok);
  if (!ok) {
    GLint len = 0;
    glGetProgramiv(program, GL_INFO_LOG_LENGTH, &len);
    std::vector<char> log(static_cast<size_t>(len > 1 ? len : 1));
    glGetProgramInfoLog(program, static_cast<GLsizei>(log.size()), nullptr, log.data());
    std::printf("Program link failed:\n%s\n", log.data());
    glDeleteProgram(program);
    return 0;
  }
  return program;
}

bool WritePpm(const char* path, int width, int height, const std::vector<uint8_t>& rgba) {
  if (!path) return false;
  std::FILE* f = std::fopen(path, "wb");
  if (!f) return false;

  std::fprintf(f, "P6\n%d %d\n255\n", width, height);

  for (int y = height - 1; y >= 0; --y) {
    const uint8_t* row = rgba.data() + (static_cast<size_t>(y) * width * 4);
    for (int x = 0; x < width; ++x) {
      const uint8_t* px = row + static_cast<size_t>(x) * 4;
      std::fwrite(px, 1, 3, f);
    }
  }

  std::fclose(f);
  return true;
}

void TryReadGpuQuery(
    const TimerQueryExt& ext,
    const GLuint* queries,
    int warmup,
    int frameIndex,
    std::vector<double>& gpuUs,
    bool force) {
  if (frameIndex < warmup) return;
  const int outIndex = frameIndex - warmup;
  if (outIndex < 0 || outIndex >= static_cast<int>(gpuUs.size())) return;
  if (gpuUs[outIndex] >= 0.0) return;

  const GLuint queryId = queries[frameIndex % 3];
  GLuint available = 0;
  ext.GetQueryObjectuivEXT(queryId, GL_QUERY_RESULT_AVAILABLE_EXT, &available);
  if (!available && !force) return;

  GLboolean disjoint = GL_FALSE;
  glGetBooleanv(GL_GPU_DISJOINT_EXT, &disjoint);
  if (disjoint == GL_TRUE) {
    gpuUs[outIndex] = -1.0;
    return;
  }

  GLuint64 ns = 0;
  ext.GetQueryObjectui64vEXT(queryId, GL_QUERY_RESULT_EXT, &ns);
  gpuUs[outIndex] = static_cast<double>(ns) / 1000.0;
}

bool WritePerfCsv(
    const std::string& path,
    const std::vector<double>& cpuSubmitUs,
    const std::vector<double>& cpuFrameUs,
    const std::vector<double>& gpuUs,
    bool hasGpu,
    double glBindBufferNsPerCall) {
  double sumSubmit = 0.0;
  double sumFrame = 0.0;
  double sumGpu = 0.0;
  int gpuCount = 0;

  for (size_t i = 0; i < cpuSubmitUs.size(); ++i) {
    sumSubmit += cpuSubmitUs[i];
    sumFrame += cpuFrameUs[i];
    if (gpuUs[i] >= 0.0) {
      sumGpu += gpuUs[i];
      gpuCount++;
    }
  }

  const double avgSubmit = cpuSubmitUs.empty() ? 0.0 : sumSubmit / cpuSubmitUs.size();
  const double avgFrame = cpuFrameUs.empty() ? 0.0 : sumFrame / cpuFrameUs.size();
  const double avgGpu = (hasGpu && gpuCount > 0) ? (sumGpu / gpuCount) : -1.0;

  std::ofstream out(path.c_str(), std::ios::out | std::ios::trunc);
  if (!out) return false;

  out << "frame,cpu_submit_us,cpu_frame_us,gpu_us\n";
  out << std::fixed << std::setprecision(3);
  for (size_t i = 0; i < cpuSubmitUs.size(); ++i) {
    out << i << "," << cpuSubmitUs[i] << "," << cpuFrameUs[i] << ",";
    if (gpuUs[i] >= 0.0)
      out << gpuUs[i];
    else
      out << "N/A";
    out << "\n";
  }

  out << "avg," << avgSubmit << "," << avgFrame << ",";
  if (avgGpu >= 0.0)
    out << avgGpu;
  else
    out << "N/A";
  out << "\n";

  if (!std::isnan(glBindBufferNsPerCall) && glBindBufferNsPerCall > 0.0) {
    out << "cpp_glBindBuffer_ns_per_call," << glBindBufferNsPerCall << ",,\n";
  } else {
    out << "cpp_glBindBuffer_ns_per_call,N/A,,\n";
  }
  return true;
}

double MeasureGlBindBufferNsPerCallBaselineSubtract(int iterations) {
  if (iterations <= 0) return std::numeric_limits<double>::quiet_NaN();

  // Warmup
  for (int i = 0; i < 1000; ++i) {
    glBindBuffer(GL_ARRAY_BUFFER, 0);
  }

  volatile int sink = 0;
  std::chrono::steady_clock::duration bestDelta = std::chrono::steady_clock::duration::max();

  for (int rep = 0; rep < 7; ++rep) {
    auto t0 = std::chrono::steady_clock::now();
    for (int i = 0; i < iterations; ++i) {
      sink ^= i;
    }
    auto t1 = std::chrono::steady_clock::now();

    auto t2 = std::chrono::steady_clock::now();
    for (int i = 0; i < iterations; ++i) {
      sink ^= i;
      glBindBuffer(GL_ARRAY_BUFFER, 0);
    }
    auto t3 = std::chrono::steady_clock::now();

    auto baseline = (t1 - t0);
    auto withCall = (t3 - t2);
    if (withCall <= baseline) continue;
    auto delta = withCall - baseline;
    if (delta < bestDelta) bestDelta = delta;
  }

  (void)sink;
  if (bestDelta == std::chrono::steady_clock::duration::max())
    return std::numeric_limits<double>::quiet_NaN();

  return ToNanoseconds(bestDelta) / static_cast<double>(iterations);
}

int RunPerf(int width, int height, int warmup, int frames, int work, bool noGpuTimer, const char* csvPath, int glbench) {
  auto h = AB_CreateOffscreenContext(3, width, height);
  if (!h) {
    std::printf("Create context failed\n");
    return 1;
  }

  const char* vs =
      "#version 300 es\n"
      "layout(location = 0) in vec2 aPos;\n"
      "void main() {\n"
      "  gl_Position = vec4(aPos, 0.0, 1.0);\n"
      "}\n";

  const char* fs =
      "#version 300 es\n"
      "precision mediump float;\n"
      "out vec4 FragColor;\n"
      "void main() {\n"
      "  FragColor = vec4(0.95, 0.25, 0.20, 1.0);\n"
      "}\n";

  GLuint vshader = CompileShader(GL_VERTEX_SHADER, vs);
  GLuint fshader = CompileShader(GL_FRAGMENT_SHADER, fs);
  if (!vshader || !fshader) {
    AB_DestroyContext(h);
    return 1;
  }

  GLuint program = LinkProgram(vshader, fshader);
  glDeleteShader(vshader);
  glDeleteShader(fshader);
  if (!program) {
    AB_DestroyContext(h);
    return 1;
  }

  const float vertices[] = {
    -0.60f, -0.40f,
     0.60f, -0.40f,
     0.00f,  0.60f,
  };

  GLuint vbo = 0;
  GLuint vao = 0;
  glGenBuffers(1, &vbo);
  glBindBuffer(GL_ARRAY_BUFFER, vbo);
  glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

  glGenVertexArrays(1, &vao);
  glBindVertexArray(vao);
  glEnableVertexAttribArray(0);
  glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), nullptr);

  TimerQueryExt timerExt = LoadTimerQueryExt();
  const bool hasTimer = timerExt.Available() && !noGpuTimer;

  GLuint queries[3] = {0, 0, 0};
  if (hasTimer) {
    timerExt.GenQueriesEXT(1, &queries[0]);
    timerExt.GenQueriesEXT(1, &queries[1]);
    timerExt.GenQueriesEXT(1, &queries[2]);
  }

  const int totalFrames = warmup + frames;
  std::vector<double> cpuSubmitUs(frames);
  std::vector<double> cpuFrameUs(frames);
  std::vector<double> gpuUs(frames, -1.0);

  const double glBindBufferNsPerCall = MeasureGlBindBufferNsPerCallBaselineSubtract(glbench);

  for (int i = 0; i < totalFrames; ++i) {
    const auto frameStart = std::chrono::steady_clock::now();

    if (hasTimer && i >= 3) {
      TryReadGpuQuery(timerExt, queries, warmup, i - 3, gpuUs, true);
    }

    const auto submitStart = std::chrono::steady_clock::now();
    if (hasTimer) timerExt.BeginQueryEXT(GL_TIME_ELAPSED_EXT, queries[i % 3]);

    for (int j = 0; j < work; ++j) {
      glViewport(0, 0, width, height);
      glClearColor(0.08f, 0.08f, 0.10f, 1.0f);
      glClear(GL_COLOR_BUFFER_BIT);

      glUseProgram(program);
      glBindVertexArray(vao);
      glDrawArrays(GL_TRIANGLES, 0, 3);
    }

    if (hasTimer) timerExt.EndQueryEXT(GL_TIME_ELAPSED_EXT);
    const auto submitEnd = std::chrono::steady_clock::now();

    if (hasTimer && i >= 2) {
      TryReadGpuQuery(timerExt, queries, warmup, i - 2, gpuUs, false);
    }

    const auto frameEnd = std::chrono::steady_clock::now();

    if (i >= warmup) {
      const int idx = i - warmup;
      cpuSubmitUs[idx] = ToMicroseconds(submitEnd - submitStart);
      cpuFrameUs[idx] = ToMicroseconds(frameEnd - frameStart);
    }
  }

  if (hasTimer) {
    glFinish();
    TryReadGpuQuery(timerExt, queries, warmup, totalFrames - 3, gpuUs, true);
    TryReadGpuQuery(timerExt, queries, warmup, totalFrames - 2, gpuUs, true);
    TryReadGpuQuery(timerExt, queries, warmup, totalFrames - 1, gpuUs, true);

    timerExt.DeleteQueriesEXT(1, &queries[0]);
    timerExt.DeleteQueriesEXT(1, &queries[1]);
    timerExt.DeleteQueriesEXT(1, &queries[2]);
  }

  if (const auto parent = std::filesystem::path(csvPath).parent_path(); !parent.empty()) {
    std::error_code ec;
    std::filesystem::create_directories(parent, ec);
  }

  const bool ok = WritePerfCsv(csvPath, cpuSubmitUs, cpuFrameUs, gpuUs, hasTimer, glBindBufferNsPerCall);

  glDeleteVertexArrays(1, &vao);
  glDeleteBuffers(1, &vbo);
  glDeleteProgram(program);
  AB_DestroyContext(h);
  return ok ? 0 : 1;
}

}  // namespace

int main(int argc, char** argv) {
  const char* outPath = "demo3_cpp.ppm";
  const char* csvPath = nullptr;
  int width = 256;
  int height = 256;
  int warmup = 200;
  int frames = 1000;
  bool perf = false;
  int work = 1;
  bool noGpuTimer = false;
  int glbench = 5 * 1000 * 1000;

  for (int i = 1; i < argc; ++i) {
    if (std::strcmp(argv[i], "--out") == 0 && i + 1 < argc) {
      outPath = argv[++i];
    } else if (std::strcmp(argv[i], "--csv") == 0 && i + 1 < argc) {
      csvPath = argv[++i];
    } else if (std::strcmp(argv[i], "--w") == 0 && i + 1 < argc) {
      width = std::atoi(argv[++i]);
    } else if (std::strcmp(argv[i], "--h") == 0 && i + 1 < argc) {
      height = std::atoi(argv[++i]);
    } else if (std::strcmp(argv[i], "--warmup") == 0 && i + 1 < argc) {
      warmup = std::atoi(argv[++i]);
    } else if (std::strcmp(argv[i], "--frames") == 0 && i + 1 < argc) {
      frames = std::atoi(argv[++i]);
    } else if (std::strcmp(argv[i], "--perf") == 0) {
      perf = true;
    } else if (std::strcmp(argv[i], "--work") == 0 && i + 1 < argc) {
      work = std::atoi(argv[++i]);
    } else if (std::strcmp(argv[i], "--no-gpu-timer") == 0) {
      noGpuTimer = true;
    } else if (std::strcmp(argv[i], "--glbench") == 0 && i + 1 < argc) {
      glbench = std::atoi(argv[++i]);
    }
  }

  if (perf) {
    if (!csvPath) csvPath = "demo3_cpp_perf.csv";
    if (work <= 0) work = 1;
    if (glbench <= 0) glbench = 1000;
    return RunPerf(width, height, warmup, frames, work, noGpuTimer, csvPath, glbench);
  }

  auto h = AB_CreateOffscreenContext(3, width, height);
  if (!h) {
    std::printf("Create context failed\n");
    return 1;
  }

  const char* vs =
      "#version 300 es\n"
      "layout(location = 0) in vec2 aPos;\n"
      "void main() {\n"
      "  gl_Position = vec4(aPos, 0.0, 1.0);\n"
      "}\n";

  const char* fs =
      "#version 300 es\n"
      "precision mediump float;\n"
      "out vec4 FragColor;\n"
      "void main() {\n"
      "  FragColor = vec4(0.25, 0.5, 0.90, 1.0);\n"
      "}\n";

  GLuint vshader = CompileShader(GL_VERTEX_SHADER, vs);
  GLuint fshader = CompileShader(GL_FRAGMENT_SHADER, fs);
  if (!vshader || !fshader) {
    AB_DestroyContext(h);
    return 1;
  }

  GLuint program = LinkProgram(vshader, fshader);
  glDeleteShader(vshader);
  glDeleteShader(fshader);
  if (!program) {
    AB_DestroyContext(h);
    return 1;
  }

  const float vertices[] = {
    -0.60f, -0.40f,
     0.60f, -0.40f,
     0.00f,  0.60f,
  };

  GLuint vbo = 0;
  GLuint vao = 0;
  glGenBuffers(1, &vbo);
  glBindBuffer(GL_ARRAY_BUFFER, vbo);
  glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

  glGenVertexArrays(1, &vao);
  glBindVertexArray(vao);
  glEnableVertexAttribArray(0);
  glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), nullptr);

  glViewport(0, 0, width, height);
  glClearColor(0.08f, 0.08f, 0.10f, 1.0f);
  glClear(GL_COLOR_BUFFER_BIT);

  glUseProgram(program);
  glBindVertexArray(vao);
  glDrawArrays(GL_TRIANGLES, 0, 3);

  glFinish();

  std::vector<uint8_t> rgba(static_cast<size_t>(width) * height * 4);
  glReadPixels(0, 0, width, height, GL_RGBA, GL_UNSIGNED_BYTE, rgba.data());

  if (const auto parent = std::filesystem::path(outPath).parent_path(); !parent.empty()) {
    std::error_code ec;
    std::filesystem::create_directories(parent, ec);
  }

  const bool ok = WritePpm(outPath, width, height, rgba);
  if (!ok) {
    std::printf("Write failed: %s\n", outPath);
  } else {
    std::printf("Wrote: %s\n", outPath);
  }

  glDeleteVertexArrays(1, &vao);
  glDeleteBuffers(1, &vbo);
  glDeleteProgram(program);
  AB_DestroyContext(h);
  return ok ? 0 : 1;
}
