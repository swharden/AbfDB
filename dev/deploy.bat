::rmdir /s /q ..\src\AbfDB.Monitor\bin
::dotnet build --configuration Release ..\src

::rmdir X:\Software\AbfDB\Watcher
::robocopy ..\src\AbfDB.Monitor\bin\Release\net6.0-windows X:\Software\AbfDB\Watcher /E

dotnet build --configuration Release ..\src

rmdir X:\Software\AbfDB\Builder
robocopy ..\src\AbfDB\bin\Release\net5.0-windows X:\Software\AbfDB\Builder /E /NJH /NFL /NDL

pause