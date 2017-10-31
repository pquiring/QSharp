dotnet build
dotnet run -p ..\..\cs2cpp . testgl1.cpp testgl1.hpp --main=TriangleWindow --ref=..\..\classlib\obj\Debug\netstandard2.0\classlib.dll %*
cmake .
make
