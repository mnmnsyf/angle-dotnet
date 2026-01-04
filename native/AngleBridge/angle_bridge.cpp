#include "angle_bridge.h"
#include <EGL/egl.h>
#include <EGL/eglext.h>
#include <EGL/eglext_angle.h>
#include <GLES2/gl2.h>

struct AB_Ctx {
  EGLDisplay display = EGL_NO_DISPLAY;
  EGLConfig  config  = nullptr;
  EGLContext context = EGL_NO_CONTEXT;
  EGLSurface surface = EGL_NO_SURFACE;
};

static EGLBoolean chooseConfig(EGLDisplay dpy, int glesMajor, EGLint surfaceType, EGLConfig* outCfg) {
  const EGLint renderable = (glesMajor >= 3) ? EGL_OPENGL_ES3_BIT : EGL_OPENGL_ES2_BIT;
  const EGLint attrs[] = {
    EGL_SURFACE_TYPE,    surfaceType,
    EGL_RENDERABLE_TYPE, renderable,
    EGL_RED_SIZE, 8, EGL_GREEN_SIZE, 8, EGL_BLUE_SIZE, 8, EGL_ALPHA_SIZE, 8,
    EGL_DEPTH_SIZE, 24, EGL_STENCIL_SIZE, 8,
    EGL_NONE
  };
  EGLint num = 0;
  if (!eglChooseConfig(dpy, attrs, outCfg, 1, &num)) return EGL_FALSE;
  return num > 0 ? EGL_TRUE : EGL_FALSE;
}

static EGLint mapBackend(int backend) {
  switch (backend) {
    case AB_BACKEND_D3D11: return EGL_PLATFORM_ANGLE_TYPE_D3D11_ANGLE;
    case AB_BACKEND_D3D9: return EGL_PLATFORM_ANGLE_TYPE_D3D9_ANGLE;
    case AB_BACKEND_VULKAN: return EGL_PLATFORM_ANGLE_TYPE_VULKAN_ANGLE;
    case AB_BACKEND_OPENGL: return EGL_PLATFORM_ANGLE_TYPE_OPENGL_ANGLE;
    case AB_BACKEND_OPENGLES: return EGL_PLATFORM_ANGLE_TYPE_OPENGLES_ANGLE;
    case AB_BACKEND_METAL: return EGL_PLATFORM_ANGLE_TYPE_METAL_ANGLE;
    case AB_BACKEND_NULL: return EGL_PLATFORM_ANGLE_TYPE_NULL_ANGLE;
    default: return EGL_PLATFORM_ANGLE_TYPE_DEFAULT_ANGLE;
  }
}

static EGLint mapDeviceType(int deviceType) {
  switch (deviceType) {
    case AB_DEVICE_HARDWARE: return EGL_PLATFORM_ANGLE_DEVICE_TYPE_HARDWARE_ANGLE;
    case AB_DEVICE_WARP: return EGL_PLATFORM_ANGLE_DEVICE_TYPE_D3D_WARP_ANGLE;
    case AB_DEVICE_REFERENCE: return EGL_PLATFORM_ANGLE_DEVICE_TYPE_D3D_REFERENCE_ANGLE;
    case AB_DEVICE_SWIFTSHADER: return EGL_PLATFORM_ANGLE_DEVICE_TYPE_SWIFTSHADER_ANGLE;
    case AB_DEVICE_NULL: return EGL_PLATFORM_ANGLE_DEVICE_TYPE_NULL_ANGLE;
    default: return EGL_PLATFORM_ANGLE_DEVICE_TYPE_HARDWARE_ANGLE;
  }
}

static EGLDisplay getDisplayWithParams(int backend, int deviceType) {
  const bool usePlatform =
      backend != AB_BACKEND_DEFAULT || deviceType != AB_DEVICE_DEFAULT;
  if (!usePlatform) {
    return eglGetDisplay(EGL_DEFAULT_DISPLAY);
  }

  auto getPlatformDisplay = reinterpret_cast<PFNEGLGETPLATFORMDISPLAYEXTPROC>(
      eglGetProcAddress("eglGetPlatformDisplayEXT"));
  if (!getPlatformDisplay) {
    return eglGetDisplay(EGL_DEFAULT_DISPLAY);
  }

  EGLint attrs[8];
  int idx = 0;
  attrs[idx++] = EGL_PLATFORM_ANGLE_TYPE_ANGLE;
  attrs[idx++] = mapBackend(backend);
  if (deviceType != AB_DEVICE_DEFAULT) {
    attrs[idx++] = EGL_PLATFORM_ANGLE_DEVICE_TYPE_ANGLE;
    attrs[idx++] = mapDeviceType(deviceType);
  }
  attrs[idx++] = EGL_NONE;

  return getPlatformDisplay(EGL_PLATFORM_ANGLE_ANGLE, EGL_DEFAULT_DISPLAY, attrs);
}

static AB_Handle createContextInternal(EGLNativeWindowType nativeWindow,
                                       bool windowSurface,
                                       int glesMajor,
                                       int width,
                                       int height,
                                       int backend,
                                       int deviceType,
                                       int swapInterval) {
  auto* ctx = new AB_Ctx();

  ctx->display = getDisplayWithParams(backend, deviceType);
  if (ctx->display == EGL_NO_DISPLAY) { delete ctx; return nullptr; }
  if (!eglInitialize(ctx->display, nullptr, nullptr)) { delete ctx; return nullptr; }

  const EGLint surfaceType = windowSurface ? EGL_WINDOW_BIT : EGL_PBUFFER_BIT;
  if (!chooseConfig(ctx->display, glesMajor, surfaceType, &ctx->config)) {
    eglTerminate(ctx->display); delete ctx; return nullptr;
  }

  if (windowSurface) {
    ctx->surface = eglCreateWindowSurface(
        ctx->display, ctx->config, nativeWindow, nullptr);
  } else {
    const EGLint pbAttrs[] = { EGL_WIDTH, width, EGL_HEIGHT, height, EGL_NONE };
    ctx->surface = eglCreatePbufferSurface(ctx->display, ctx->config, pbAttrs);
  }
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

  if (windowSurface && swapInterval >= 0 && !eglSwapInterval(ctx->display, swapInterval)) {
    eglDestroyContext(ctx->display, ctx->context);
    eglDestroySurface(ctx->display, ctx->surface);
    eglTerminate(ctx->display); delete ctx; return nullptr;
  }

  return (AB_Handle)ctx;
}

AB_Handle AB_CreateOffscreenContext(int glesMajor, int width, int height) {
  return AB_CreateOffscreenContextEx(
      glesMajor, width, height, AB_BACKEND_DEFAULT, AB_DEVICE_DEFAULT, -1);
}

AB_Handle AB_CreateOffscreenContextEx(int glesMajor,
                                      int width,
                                      int height,
                                      int backend,
                                      int deviceType,
                                      int swapInterval) {
  return createContextInternal(
      0, false, glesMajor, width, height, backend, deviceType, swapInterval);
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
  return AB_CreateWindowContextEx(
      nativeWindow, glesMajor, AB_BACKEND_DEFAULT, AB_DEVICE_DEFAULT, -1);
}

AB_Handle AB_CreateWindowContextEx(void* nativeWindow,
                                   int glesMajor,
                                   int backend,
                                   int deviceType,
                                   int swapInterval) {
  return createContextInternal((EGLNativeWindowType)nativeWindow, true, glesMajor, 0, 0,
                               backend, deviceType, swapInterval);
}

int AB_SwapBuffers(AB_Handle h) {
  auto* ctx = (AB_Ctx*)h;
  return eglSwapBuffers(ctx->display, ctx->surface) ? 1 : 0;
}

static PFNEGLPOSTSUBBUFFERNVPROC g_postSubBufferNV = nullptr;

int AB_PostSubBufferNV(AB_Handle h, int x, int y, int width, int height) {
  auto* ctx = (AB_Ctx*)h;
  if (!g_postSubBufferNV) {
    g_postSubBufferNV = reinterpret_cast<PFNEGLPOSTSUBBUFFERNVPROC>(
        eglGetProcAddress("eglPostSubBufferNV"));
    if (!g_postSubBufferNV) {
      return 0;
    }
  }

  return g_postSubBufferNV(ctx->display, ctx->surface, x, y, width, height) ? 1 : 0;
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
