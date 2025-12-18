#include "angle_bridge.h"
#include <cstdio>
#include <GLES2/gl2.h>

int main() {
  auto h = AB_CreateOffscreenContext(3, 16, 16);
  if (!h) { std::printf("Create context failed\n"); return 1; }

  std::printf("GL_VERSION: %s\n", AB_GetGLString(GL_VERSION));
  AB_DestroyContext(h);
  return 0;
}
