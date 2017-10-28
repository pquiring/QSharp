dotnet build
dotnet run -p ..\..\cs2cpp . testunsafe.cpp testunsafe.hpp --main=testunsafe.Program --ref=..\..\classlib\bin\Debug\netstandard2.0\classlib.dll %*
cmake .
make
