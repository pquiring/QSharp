dotnet build
dotnet run -p ..\cs2cpp Qt classlib.cpp classlib.hpp --library --classlib %*
cmake .
make
