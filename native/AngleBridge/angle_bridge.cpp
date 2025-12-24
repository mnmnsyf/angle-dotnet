#include "angle_bridge.h"
#include <EGL/egl.h>
#include <GLES2/gl2.h>

struct AB_Ctx {
  EGLDisplay display = EGL_NO_DISPLAY;
  EGLConfig  config  = nullptr;
  EGLContext context = EGL_NO_CONTEXT;
  EGLSurface surface = EGL_NO_SURFACE;
};

static EGLBoolean chooseConfig(EGLDisplay dpy, int glesMajor, EGLConfig* outCfg) {
  const EGLint renderable = (glesMajor >= 3) ? EGL_OPENGL_ES3_BIT : EGL_OPENGL_ES2_BIT;
  const EGLint attrs[] = {
    EGL_SURFACE_TYPE,    EGL_PBUFFER_BIT,
    EGL_RENDERABLE_TYPE, renderable,
    EGL_RED_SIZE, 8, EGL_GREEN_SIZE, 8, EGL_BLUE_SIZE, 8, EGL_ALPHA_SIZE, 8,
    EGL_DEPTH_SIZE, 24, EGL_STENCIL_SIZE, 8,
    EGL_NONE
  };
  EGLint num = 0;
  if (!eglChooseConfig(dpy, attrs, outCfg, 1, &num)) return EGL_FALSE;
  return num > 0 ? EGL_TRUE : EGL_FALSE;
}

AB_Handle AB_CreateOffscreenContext(int glesMajor, int width, int height) {
  auto* ctx = new AB_Ctx();

  ctx->display = eglGetDisplay(EGL_DEFAULT_DISPLAY);
  if (ctx->display == EGL_NO_DISPLAY) { delete ctx; return nullptr; }
  if (!eglInitialize(ctx->display, nullptr, nullptr)) { delete ctx; return nullptr; }

  if (!chooseConfig(ctx->display, glesMajor, &ctx->config)) {
    eglTerminate(ctx->display); delete ctx; return nullptr;
  }

  const EGLint pbAttrs[] = { EGL_WIDTH, width, EGL_HEIGHT, height, EGL_NONE };
  ctx->surface = eglCreatePbufferSurface(ctx->display, ctx->config, pbAttrs);
  if (ctx->surface == EGL_NO_SURFACE) {
    eglTerminate(ctx->display); delete ctx; return nullptr;
  }

  const EGLint ctxAttrs[] = { EGL_CONTEXT_CLIENT_VERSION, glesMajor, EGL_NONE };
  ctx->context = eglCreateContext(ctx->display, ctx->config, EGL_NO_CONTEXT, ctxAttrs);
  if (ctx->context == EGL_NO_CONTEXT) {
    eglDestroySurface(ctx->display, ctx->surface);
    eglTerminate(ctx->display); delete ctx; return nullptr;
  }

  if (!eglMakeCurrent(ctx->display, ctx->surface, ctx->surface, ctx->context)) {
    eglDestroyContext(ctx->display, ctx->context);
    eglDestroySurface(ctx->display, ctx->surface);
    eglTerminate(ctx->display); delete ctx; return nullptr;
  }

  return (AB_Handle)ctx;
}

void AB_DestroyContext(AB_Handle h) {
  if (!h) return;
  auto* ctx = (AB_Ctx*)h;

  eglMakeCurrent(ctx->display, EGL_NO_SURFACE, EGL_NO_SURFACE, EGL_NO_CONTEXT);
  if (ctx->context != EGL_NO_CONTEXT) eglDestroyContext(ctx->display, ctx->context);
  if (ctx->surface != EGL_NO_SURFACE) eglDestroySurface(ctx->display, ctx->surface);
  if (ctx->display != EGL_NO_DISPLAY) eglTerminate(ctx->display);
  delete ctx;
}

const char* AB_GetGLString(int name) {
  const GLubyte* s = glGetString((GLenum)name);
  return (const char*)s;
}

void* AB_GetProcAddress(const char* name) {
  return reinterpret_cast<void*>(eglGetProcAddress(name));
}

AB_Handle AB_CreateWindowContext(void* nativeWindow, int glesMajor) {
  auto* ctx = new AB_Ctx();
  ctx->display = eglGetDisplay(EGL_DEFAULT_DISPLAY);
  if (ctx->display == EGL_NO_DISPLAY) { delete ctx; return nullptr; }
  if (!eglInitialize(ctx->display, nullptr, nullptr)) { delete ctx; return nullptr; }

  if (!chooseConfig(ctx->display, glesMajor, &ctx->config)) {
    eglTerminate(ctx->display); delete ctx; return nullptr;
  }

  // 上屏：window surface
  ctx->surface = eglCreateWindowSurface(
      ctx->display, ctx->config, (EGLNativeWindowType)nativeWindow, nullptr);
  if (ctx->surface == EGL_NO_SURFACE) {
    eglTerminate(ctx->display); delete ctx; return nullptr;
  }

  const EGLint ctxAttrs[] = { EGL_CONTEXT_CLIENT_VERSION, glesMajor, EGL_NONE };
  ctx->context = eglCreateContext(ctx->display, ctx->config, EGL_NO_CONTEXT, ctxAttrs);
  if (ctx->context == EGL_NO_CONTEXT) {
    eglDestroySurface(ctx->display, ctx->surface);
    eglTerminate(ctx->display); delete ctx; return nullptr;
  }

  if (!eglMakeCurrent(ctx->display, ctx->surface, ctx->surface, ctx->context)) {
    eglDestroyContext(ctx->display, ctx->context);
    eglDestroySurface(ctx->display, ctx->surface);
    eglTerminate(ctx->display); delete ctx; return nullptr;
  }
  return (AB_Handle)ctx;
}

int AB_SwapBuffers(AB_Handle h) {
  auto* ctx = (AB_Ctx*)h;
  return eglSwapBuffers(ctx->display, ctx->surface) ? 1 : 0;
}

static volatile uint64_t g_noop_sink = 0;

void AB_NoOp() {
}

void AB_NoOpN(int n) {
  if (n <= 0) return;
  for (int i = 0; i < n; ++i) {
    g_noop_sink += static_cast<uint64_t>(i);
  }
}

void AB_GlCallBatch(int n) {
  if (n <= 0) return;
  for (int i = 0; i < n; ++i) {
    glBindBuffer(GL_ARRAY_BUFFER, 0);
  }
}
