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

AB_EXPORT AB_Handle AB_CreateOffscreenContext(int glesMajor, int width, int height);
AB_EXPORT void      AB_DestroyContext(AB_Handle h);
AB_EXPORT const char* AB_GetGLString(int name);

AB_EXPORT AB_Handle AB_CreateWindowContext(void* nativeWindow, int glesMajor);
AB_EXPORT int      AB_SwapBuffers(AB_Handle h);
AB_EXPORT void*    AB_GetProcAddress(const char* name);


#ifdef __cplusplus
}
#endif
