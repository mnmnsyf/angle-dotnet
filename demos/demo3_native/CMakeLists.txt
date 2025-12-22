add_executable(demo3_native
  main.cpp
)

target_link_libraries(demo3_native PRIVATE
  AngleBridge
)

if (WIN32)
  set(_angle_gles2_candidates
    "${ANGLE_OUT}/libGLESv2.lib"
    "${ANGLE_OUT}/libGLESv2.dll.lib"
    "${ANGLE_OUT}/obj/libGLESv2_static.lib"
    "${ANGLE_OUT}/obj/libGLESv2_thin_static.lib"
  )

  foreach(_path IN LISTS _angle_gles2_candidates)
    if(EXISTS "${_path}")
      set(ANGLE_GLES2_LIB "${_path}")
      break()
    endif()
  endforeach()

  if(NOT ANGLE_GLES2_LIB)
    message(FATAL_ERROR
      "Could not find ANGLE GLES2 import/static libraries under ANGLE_OUT='${ANGLE_OUT}'.\n"
      "Tried: ${_angle_gles2_candidates}\n"
    )
  endif()

  target_link_libraries(demo3_native PRIVATE
    "${ANGLE_GLES2_LIB}"
  )

  file(GLOB _angle_runtime_dlls CONFIGURE_DEPENDS
    "${ANGLE_OUT}/*.dll"
  )

  add_custom_command(TARGET demo3_native POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy_if_different
      $<TARGET_FILE:AngleBridge>
      $<TARGET_FILE_DIR:demo3_native>
    COMMAND ${CMAKE_COMMAND} -E copy_if_different
      ${_angle_runtime_dlls}
      $<TARGET_FILE_DIR:demo3_native>
  )
endif()
