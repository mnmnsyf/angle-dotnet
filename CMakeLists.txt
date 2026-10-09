cmake_minimum_required(VERSION 3.20)
project(AngleSharpGLES LANGUAGES C CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Pass these values during configuration or define them in CMakePresets.json:
# -DANGLE_ROOT=... (path to the ANGLE repository)
# -DANGLE_OUT=...  (ANGLE GN/Ninja output directory, such as external/angle/out/Release)
if(NOT DEFINED ANGLE_ROOT)
  message(FATAL_ERROR "Please set -DANGLE_ROOT=... (path to the ANGLE repository)")
endif()
if(NOT DEFINED ANGLE_OUT)
  message(FATAL_ERROR "Please set -DANGLE_OUT=... (path to the ANGLE build output directory)")
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
