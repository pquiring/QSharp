@echo off
::build the .NET Project (optional)
dotnet build
::generate the cpp sources and CMakeLists.txt
..\bin\cs2cpp.exe Qt classlib --library --classlib --home=.. %QSHARP% %*
