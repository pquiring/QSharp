dotnet publish -c Release -r win10-x64
copy bin\Release\netcoreapp2.0\win10-x64 ..\bin
md ..\bin\publish
copy bin\Release\netcoreapp2.0\win10-x64\publish ..\bin\publish
