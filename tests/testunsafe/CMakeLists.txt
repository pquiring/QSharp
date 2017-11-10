cmake_minimum_required(VERSION 3.6)

set (CMAKE_CXX_STANDARD 14)

include_directories(/usr/include/qt5)

include_directories(../../include)

link_directories(../../lib)

add_executable(testunsafe testunsafe.cpp)

target_link_libraries(testunsafe classlib Qt5Core Qt5Gui Qt5Network Qt5Widgets stdc++ z)

