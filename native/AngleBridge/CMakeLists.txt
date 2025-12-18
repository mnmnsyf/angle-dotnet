add_library(AngleBridge SHARED
  angle_bridge.cpp
)

target_include_directories(AngleBridge
  PUBLIC
    ${CMAKE_CURRENT_SOURCE_DIR}
    ${ANGLE_ROOT}/include
)

if (WIN32)
  set(_angle_egl_candidates
    "${ANGLE_OUT}/libEGL.lib"
    "${ANGLE_OUT}/libEGL.dll.lib"
    "${ANGLE_OUT}/obj/libEGL_static.lib"
  )
  set(_angle_gles2_candidates
    "${ANGLE_OUT}/libGLESv2.lib"
    "${ANGLE_OUT}/libGLESv2.dll.lib"
    "${ANGLE_OUT}/obj/libGLESv2_static.lib"
    "${ANGLE_OUT}/obj/libGLESv2_thin_static.lib"
  )

  foreach(_path IN LISTS _angle_egl_candidates)
    if(EXISTS "${_path}")
      set(ANGLE_EGL_LIB "${_path}")
      break()
    endif()
  endforeach()

  foreach(_path IN LISTS _angle_gles2_candidates)
    if(EXISTS "${_path}")
      set(ANGLE_GLES2_LIB "${_path}")
      break()
    endif()
  endforeach()

  if(NOT ANGLE_EGL_LIB OR NOT ANGLE_GLES2_LIB)
    message(FATAL_ERROR
      "Could not find ANGLE import/static libraries under ANGLE_OUT='${ANGLE_OUT}'.\n"
      "Tried EGL: ${_angle_egl_candidates}\n"
      "Tried GLES2: ${_angle_gles2_candidates}\n"
      "On Windows GN/Ninja builds usually produce 'libEGL.dll.lib' and 'libGLESv2.dll.lib'.\n"
      "Make sure you've built ANGLE first (e.g. 'autoninja -C ${ANGLE_OUT} libEGL libGLESv2')."
    )
  endif()

  target_link_libraries(AngleBridge PRIVATE
    "${ANGLE_EGL_LIB}"
    "${ANGLE_GLES2_LIB}"
  )
endif()
