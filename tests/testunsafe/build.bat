dotnet build
dotnet run -p ..\..\cs2cpp . testunsafe --home=..\.. --main=testunsafe.Program --ref=..\..\classlib\bin\Debug\netstandard2.0\classlib.dll %*
cmake .
make
