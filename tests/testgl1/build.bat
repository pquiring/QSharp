::dotnet build
..\..\bin\cs2cpp . testgl1 --home=..\.. --main=TriangleWindow --ref=..\..\classlib\obj\Debug\netstandard2.0\classlib.dll %*
cmake .
make
