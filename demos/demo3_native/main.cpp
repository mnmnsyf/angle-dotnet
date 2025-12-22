#include "angle_bridge.h"
#include <GLES3/gl3.h>
#include <cstdio>
#include <cstdlib>
#include <cstring>
#include <filesystem>
#include <vector>

namespace {

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

}  // namespace

int main(int argc, char** argv) {
  const char* outPath = "demo3_cpp.ppm";
  int width = 256;
  int height = 256;

  for (int i = 1; i < argc; ++i) {
    if (std::strcmp(argv[i], "--out") == 0 && i + 1 < argc) {
      outPath = argv[++i];
    } else if (std::strcmp(argv[i], "--w") == 0 && i + 1 < argc) {
      width = std::atoi(argv[++i]);
    } else if (std::strcmp(argv[i], "--h") == 0 && i + 1 < argc) {
      height = std::atoi(argv[++i]);
    }
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
