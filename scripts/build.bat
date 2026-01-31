@echo off
echo Building MCPComputerUse with Official MCP SDK...

REM Clean previous builds
echo Cleaning previous builds...
if exist "src\WindowsAutomation\bin" rmdir /s /q "src\WindowsAutomation\bin"
if exist "src\WindowsAutomation\obj" rmdir /s /q "src\WindowsAutomation\obj"
if exist "src\MCPComputerUse\bin" rmdir /s /q "src\MCPComputerUse\bin"
if exist "src\MCPComputerUse\obj" rmdir /s /q "src\MCPComputerUse\obj"

REM Restore packages
echo Restoring NuGet packages...
dotnet restore MCPComputerUse.sln

if %ERRORLEVEL% NEQ 0 (
    echo Package restore failed! Trying individual projects...
    dotnet restore src/WindowsAutomation/WindowsAutomation.csproj
    dotnet restore src/MCPComputerUse/MCPComputerUse.csproj
)

REM Build solution
echo Building solution...
dotnet build MCPComputerUse.sln --configuration Release --no-restore

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✅ Build completed successfully!
    echo.
    echo 📁 Output locations:
    echo    WindowsAutomation.dll: src\WindowsAutomation\bin\Release\net9.0-windows\
    echo    MCPComputerUse.exe:     src\MCPComputerUse\bin\Release\net9.0-windows\
    echo.
    echo 🚀 Next steps:
    echo    1. Run 'scripts\publish.bat' to create single-file executable
    echo    2. Run 'scripts\install.bat' to configure Claude Desktop
    echo    3. Test with Claude Desktop integration
    echo.
    echo 💡 The build includes:
    echo    - Official ModelContextProtocol SDK
    echo    - Native Windows automation library
    echo    - 11 computer use tools
    echo    - Professional error handling
) else (
    echo.
    echo ❌ Build failed!
    echo.
    echo 🔍 Troubleshooting:
    echo    1. Ensure .NET 9 SDK is installed: dotnet --version
    echo    2. Check for missing dependencies
    echo    3. Verify ModelContextProtocol package is available
    echo    4. Try cleaning and rebuilding: dotnet clean ^&^& dotnet build
    echo.
    echo 📋 Alternative: Use the simplified version in tools/ directory
    exit /b 1
)

pause
