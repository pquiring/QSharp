dotnet build
dotnet run -p ..\..\cs2cpp . helloworld.cpp helloworld.hpp --main=MainClass --ref=..\..\classlib\bin\Debug\netstandard2.0\classlib.dll %*
cmake -D CMAKE_BUILD_TYPE=Release .
make
