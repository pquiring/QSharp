cmake_minimum_required(VERSION 3.6)

set (CMAKE_CXX_STANDARD 11)

set (CMAKE_BUILD_TYPE "Debug")

include_directories(/usr/include/qt5)

include_directories(../../include)

link_directories(../../lib)

add_executable(testgl1 testgl1.cpp)

target_link_libraries(testgl1 classlib Qt5Core Qt5Gui Qt5Network stdc++ z)
