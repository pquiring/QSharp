cmake_minimum_required(VERSION 3.6)

set (CMAKE_CXX_STANDARD 11)

set (CMAKE_BUILD_TYPE "Debug")

add_library(classlib classlib.cpp)

include_directories(/usr/include/qt5)

include_directories(../include)

add_custom_command(TARGET classlib POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy classlib.hpp ../include
)

add_custom_command(TARGET classlib POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy $<TARGET_FILE:classlib> ../lib
)
