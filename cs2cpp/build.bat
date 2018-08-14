dotnet build
dotnet publish -c Release -r win10-x64
xcopy /D /Y bin\Release\netcoreapp2.1\win10-x64 ..\bin > nul
md ..\bin\publish 2> nul
xcopy /D /Y bin\Release\netcoreapp2.1\win10-x64\publish ..\bin\publish > nul
