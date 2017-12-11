@echo off
dotnet build
..\bin\cs2cpp.exe Qt classlib --library --classlib --home=.. %*
cmake -D CMAKE_BUILD_TYPE=Release .
make
