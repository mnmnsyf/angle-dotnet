cmake_minimum_required(VERSION 3.20)
project(AngleSharpGLES LANGUAGES C CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# 在 configure 时传入（或者写进 CMakePresets.json 的 cacheVariables）：
# -DANGLE_ROOT=... (external/angle 的路径)
# -DANGLE_OUT=...  (你 autoninja 输出目录，比如 external/angle/out/Release)
if(NOT DEFINED ANGLE_ROOT)
  message(FATAL_ERROR "Please set -DANGLE_ROOT=... (path to angle repo root)")
endif()
if(NOT DEFINED ANGLE_OUT)
  message(FATAL_ERROR "Please set -DANGLE_OUT=... (path to angle build output dir)")
endif()

add_subdirectory(native/AngleBridge)
if(EXISTS "${CMAKE_CURRENT_SOURCE_DIR}/demos/demo1_native/CMakeLists.txt")
  add_subdirectory(demos/demo1_native)
else()
  message(STATUS "Skipping demos/demo1_native (missing CMakeLists.txt)")
endif()

if(EXISTS "${CMAKE_CURRENT_SOURCE_DIR}/demos/demo3_native/CMakeLists.txt")
  add_subdirectory(demos/demo3_native)
else()
  message(STATUS "Skipping demos/demo3_native (missing CMakeLists.txt)")
endif()
