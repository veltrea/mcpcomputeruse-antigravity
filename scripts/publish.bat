@echo off
echo Publishing MCPComputerUse with Official MCP SDK as single file executable...

REM Clean previous builds
if exist "tools\MCPComputerUse.exe" del "tools\MCPComputerUse.exe"
if exist "tools\output" rmdir /s /q "tools\output"

REM Restore packages for the main project
echo Restoring NuGet packages...
dotnet restore src/MCPComputerUse/MCPComputerUse.csproj

if %ERRORLEVEL% NEQ 0 (
    echo Package restore failed!
    pause
    exit /b 1
)

REM Publish self-contained single file executable
echo Publishing self-contained single file executable...
dotnet publish src/MCPComputerUse/MCPComputerUse.csproj ^
    -c Release ^
    -r win-x64 ^
    --self-contained true ^
    -p:PublishSingleFile=true ^
    -p:PublishTrimmed=false ^
    -p:IncludeNativeLibrariesForSelfExtract=true ^
    -p:EnableCompressionInSingleFile=true ^
    -o ./tools

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✅ Publish completed successfully!
    echo.
    echo 📁 Executable location: 
    echo    c:\LLM\Projects\ClaudeTest\MCPComputerUse\tools\MCPComputerUse.exe
    echo.
    echo 📊 File information:
    for %%F in ("tools\MCPComputerUse.exe") do echo    Size: %%~zF bytes
    dir "tools\MCPComputerUse.exe" | findstr MCPComputerUse.exe
    echo.
    echo 🚀 Features included:
    echo    - Official ModelContextProtocol SDK
    echo    - 11 Computer Use automation tools
    echo    - Native Windows API integration
    echo    - Self-contained deployment
    echo    - Professional error handling
    echo.
    echo 🔧 Ready for Claude Desktop integration!
    echo    Use this configuration in claude_desktop_config.json:
    echo.
    echo    {
    echo      "mcpServers": {
    echo        "computer-use": {
    echo          "command": "c:/LLM/Projects/ClaudeTest/MCPComputerUse/tools/MCPComputerUse.exe",
    echo          "args": []
    echo        }
    echo      }
    echo    }
    echo.
    echo 📝 Logs will be written to: %%TEMP%%\mcpcomputeruse-log.txt
    echo.
    echo ✨ MCPComputerUse is ready for production use!
) else (
    echo.
    echo ❌ Publish failed!
    echo.
    echo 🔍 Troubleshooting:
    echo    1. Ensure .NET 9 SDK is installed
    echo    2. Check internet connection for NuGet packages
    echo    3. Verify all source files are present
    echo    4. Try building first: dotnet build src/MCPComputerUse/MCPComputerUse.csproj
    echo.
    echo 📋 Alternative: Use the simplified version in tools/MCPComputerUse.cs
    exit /b 1
)

pause
