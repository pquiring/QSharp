cmake_minimum_required(VERSION 3.6)

set (CMAKE_CXX_STANDARD 14)

add_library(classlib classlib.cpp
  quazip/qioapi.cpp quazip/quaadler32.cpp quazip/quacrc32.cpp quazip/quagzipfile.cpp quazip/quaziodevice.cpp quazip/quazip.cpp quazip/quazipdir.cpp quazip/quazipfile.cpp quazip/quazipfileinfo.cpp quazip/quazipnewinfo.cpp quazip/unzip.c quazip/zip.c
  quazip/release/moc_quagzipfile.cpp quazip/release/moc_quaziodevice.cpp quazip/release/moc_quazipfile.cpp
)

include_directories(/usr/include/qt5)

include_directories(/usr/include/ffmpeg)

include_directories(../include)

include_directories(../include/quazip)

add_custom_command(TARGET classlib POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy classlib.hpp ../include
)

add_custom_command(TARGET classlib POST_BUILD
    COMMAND ${CMAKE_COMMAND} -E copy $<TARGET_FILE:classlib> ../lib
)
