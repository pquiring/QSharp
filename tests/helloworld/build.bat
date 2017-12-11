dotnet build
dotnet run -p ..\..\cs2cpp . helloworld --main=MainClass --home=..\.. --ref=..\..\classlib\bin\Debug\netstandard2.0\classlib.dll %QSHARP% %*
