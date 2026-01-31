@echo off
set "SOURCE=C:\Users\user\マイドライブ\dev\mcp\ComputerUse\source"
set "DEST=C:\Users\user\MCPComputerUse"

echo Copying MCPComputerUse source to %DEST%...
if not exist "%DEST%" mkdir "%DEST%"
xcopy /E /I /Y "%SOURCE%\*" "%DEST%"

echo Done.
pause
