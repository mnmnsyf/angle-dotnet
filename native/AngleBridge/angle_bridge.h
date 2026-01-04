#pragma once
#include <stdint.h>

#ifdef _WIN32
  #define AB_EXPORT __declspec(dllexport)
#else
  #define AB_EXPORT __attribute__((visibility("default")))
#endif

#ifdef __cplusplus
extern "C" {
#endif

typedef void* AB_Handle;

typedef enum AB_BackendType {
  AB_BACKEND_DEFAULT  = 0,
  AB_BACKEND_D3D11    = 1,
  AB_BACKEND_D3D9     = 2,
  AB_BACKEND_VULKAN   = 3,
  AB_BACKEND_OPENGL   = 4,
  AB_BACKEND_OPENGLES = 5,
  AB_BACKEND_METAL    = 6,
  AB_BACKEND_NULL     = 7,
} AB_BackendType;

typedef enum AB_DeviceType {
  AB_DEVICE_DEFAULT     = 0,
  AB_DEVICE_HARDWARE    = 1,
  AB_DEVICE_WARP        = 2,
  AB_DEVICE_REFERENCE   = 3,
  AB_DEVICE_SWIFTSHADER = 4,
  AB_DEVICE_NULL        = 5,
} AB_DeviceType;

AB_EXPORT AB_Handle AB_CreateOffscreenContext(int glesMajor, int width, int height);
AB_EXPORT AB_Handle AB_CreateOffscreenContextEx(int glesMajor, int width, int height, int backend, int deviceType, int swapInterval);
AB_EXPORT void      AB_DestroyContext(AB_Handle h);
AB_EXPORT const char* AB_GetGLString(int name);

AB_EXPORT AB_Handle AB_CreateWindowContext(void* nativeWindow, int glesMajor);
AB_EXPORT AB_Handle AB_CreateWindowContextEx(void* nativeWindow, int glesMajor, int backend, int deviceType, int swapInterval);
AB_EXPORT int      AB_SwapBuffers(AB_Handle h);
AB_EXPORT int      AB_PostSubBufferNV(AB_Handle h, int x, int y, int width, int height);
AB_EXPORT void*    AB_GetProcAddress(const char* name);
AB_EXPORT void     AB_NoOp();
AB_EXPORT void     AB_NoOpN(int n);
AB_EXPORT void     AB_GlCallBatch(int n);


#ifdef __cplusplus
}
#endif
