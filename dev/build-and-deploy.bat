echo off

echo.
echo Reuilding project using Release mode...
dotnet publish ..\src\AbfDB --configuration Release --self-contained --runtime win-x86

xcopy ..\src\AbfDB\bin\Release\net5.0\win-x86\publish "X:\Software\abfdb" /E

pause