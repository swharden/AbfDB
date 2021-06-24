echo off

echo.
echo Deleting artifacts from old runs...
del abfdb.csv
del abfdb.sqlite
del log.txt

echo.
echo Reuilding project using Release mode...
dotnet build ..\src\ --configuration Release

echo.
echo Press ENTER to begin scanning ABFs...
pause

echo.
..\src\AbfDB\bin\Release\net48\AbfDB.exe "X:\Data\AT1-Cre-AT2-eGFP"

echo.
echo Scan complete.
pause