Q# : helloworld

To compile under cygwin/mingw define these environment variables before calling cmake:

set CC=/usr/bin/x86_64-w64-mingw32-gcc.exe
set CXX=/usr/bin/x86_64-w64-mingw32-gcc.exe

Building:
 - dotnet build
 - dotnet run -p ..\..\cs2cpp . helloworld.cpp helloworld.hpp --main=MainClass --ref=..\..\classlib\bin\Debug\netstandard2.0\classlib.dll
 - cmake .
 - make
