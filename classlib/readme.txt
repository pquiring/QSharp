Q# classlib

To compile under cygwin/mingw define these environment variables before calling cmake:

set CC=/usr/bin/x86_64-w64-mingw32-gcc.exe
set CXX=/usr/bin/x86_64-w64-mingw32-gcc.exe

Q# does not contain many C++ files, instead to insert C++ code into the C# code special attributes and methods are used.
  Attribute [CPPClass(String cpp)] - used to insert C++ code directly into a class
  CPP.Add(String cpp) - used to insert code into a method
  CPP.Return...(String cpp) - return a C++ value in a method

Building:
 - dotnet build
 - dotnet run -p ..\cs2cpp Qt classlib.cpp classlib.hpp --library --classlib
 - cmake .
 - make | nmake
