@echo off
dotnet build
if exist ..\bin\cs2cpp.exe (..\bin\cs2cpp.exe Qt classlib.cpp classlib.hpp --library --classlib %*) else (dotnet run -p ..\cs2cpp Qt classlib.cpp classlib.hpp --library --classlib %*)
cmake .
make
