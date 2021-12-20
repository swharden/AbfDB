rmdir /s /q ..\src\AbfDB.Monitor\bin
dotnet build --configuration Release ..\src\AbfDB.Monitor

rmdir X:\Software\AbfDB\Watcher
robocopy ..\src\AbfDB.Monitor\bin\Release\net6.0-windows X:\Software\AbfDB\Watcher /E

pause