@echo off
set "SOURCE=C:\Users\pcadmin\マイドライブ\dev\mcp\ComputerUse\source"
set "DEST=C:\Users\pcadmin\MCPComputerUse"

echo Copying MCPComputerUse source to %DEST%...
if not exist "%DEST%" mkdir "%DEST%"
xcopy /E /I /Y "%SOURCE%\*" "%DEST%"

echo Done.
pause
