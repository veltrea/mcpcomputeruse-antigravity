@echo off
echo Installing MCPComputerUse to Claude Desktop...

set "APPDATA_CLAUDE=%APPDATA%\Claude"
set "CONFIG_FILE=%APPDATA_CLAUDE%\claude_desktop_config.json"
set "EXECUTABLE_PATH=%~dp0..\tools\MCPComputerUse.exe"

REM Check if executable exists
if not exist "%EXECUTABLE_PATH%" (
    echo Error: MCPComputerUse.exe not found at %EXECUTABLE_PATH%
    echo Please run publish.bat first to create the executable.
    pause
    exit /b 1
)

REM Create Claude directory if it doesn't exist
if not exist "%APPDATA_CLAUDE%" (
    echo Creating Claude directory...
    mkdir "%APPDATA_CLAUDE%"
)

REM Convert path to forward slashes for JSON
set "JSON_PATH=c:/LLM/Projects/ClaudeTest/MCPComputerUse/tools/MCPComputerUse.exe"

REM Create or update configuration
echo Creating Claude Desktop configuration...
(
echo {
echo   "mcpServers": {
echo     "computer-use": {
echo       "command": "%JSON_PATH%",
echo       "args": []
echo     }
echo   }
echo }
) > "%CONFIG_FILE%"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Installation completed successfully!
    echo Configuration file: %CONFIG_FILE%
    echo Executable: %JSON_PATH%
    echo.
    echo Please restart Claude Desktop to use the new MCP server.
    echo The server will be available as "computer-use" in Claude Desktop.
) else (
    echo Installation failed!
    exit /b 1
)

pause
