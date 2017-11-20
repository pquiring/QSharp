dotnet publish -c Release -r win10-x64
copy bin\Release\netcoreapp2.0\win10-x64 ..\bin > nul
md ..\bin\publish 2> nul
copy bin\Release\netcoreapp2.0\win10-x64\publish ..\bin\publish > nul
