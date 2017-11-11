dotnet build
..\..\bin\cs2cpp . testwidgets.cpp testwidgets.hpp --main=testwidgets.Program --ref=..\..\classlib\bin\Debug\netstandard2.0\classlib.dll %*
cmake .
make
