add_executable(demo1_native
  main.cpp
)

target_link_libraries(demo1_native PRIVATE
  AngleBridge
)

if (WIN32)
  file(GLOB _angle_runtime_dlls CONFIGURE_DEPENDS
    "${ANGLE_OUT}/*.dll"
  )

  add_custom_command(TARGET demo1_native POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy_if_different
      $<TARGET_FILE:AngleBridge>
      $<TARGET_FILE_DIR:demo1_native>
    COMMAND ${CMAKE_COMMAND} -E copy_if_different
      ${_angle_runtime_dlls}
      $<TARGET_FILE_DIR:demo1_native>
  )
endif()
