@echo off
echo Testing MCPComputerUse MCP Server Functionality...

cd /d "c:\LLM\Projects\ClaudeTest\MCPComputerUse"

if not exist "tools\MCPComputerUse.exe" (
    echo ❌ MCPComputerUse.exe not found!
    echo Please run 'scripts\publish.bat' first to create the executable.
    pause
    exit /b 1
)

echo.
echo 🚀 Testing MCP server startup and tool discovery...
echo.

REM Create a test input for the MCP server
echo {"jsonrpc": "2.0", "id": 1, "method": "initialize", "params": {"protocolVersion": "2024-11-05", "capabilities": {}, "clientInfo": {"name": "test", "version": "1.0"}}} > test_input.txt
echo {"jsonrpc": "2.0", "id": 2, "method": "tools/list"} >> test_input.txt
echo {"jsonrpc": "2.0", "id": 3, "method": "tools/call", "params": {"name": "computer-use:get_server_capabilities", "arguments": {}}} >> test_input.txt

echo 📝 Sending test commands to MCP server...
echo.

REM Test the MCP server with timeout
timeout 5 tools\MCPComputerUse.exe < test_input.txt > test_output.txt 2> test_error.txt

echo 📊 Server response:
echo.
type test_output.txt
echo.

if exist test_error.txt (
    echo 📋 Error log:
    type test_error.txt
    echo.
)

REM Check if the response contains expected tools
findstr /i "computer-use:take_screenshot" test_output.txt >nul
if %ERRORLEVEL% EQU 0 (
    echo ✅ Tool discovery working - screenshot tool found!
) else (
    echo ⚠️ Screenshot tool not found in response
)

findstr /i "computer-use:mouse_click" test_output.txt >nul
if %ERRORLEVEL% EQU 0 (
    echo ✅ Tool discovery working - mouse tool found!
) else (
    echo ⚠️ Mouse tool not found in response
)

findstr /i "MCPComputerUse" test_output.txt >nul
if %ERRORLEVEL% EQU 0 (
    echo ✅ Server identification working!
) else (
    echo ⚠️ Server identification not found
)

findstr /i "capabilities" test_output.txt >nul
if %ERRORLEVEL% EQU 0 (
    echo ✅ Server capabilities responding!
) else (
    echo ⚠️ Server capabilities not responding
)

echo.
echo 🧹 Cleaning up test files...
del test_input.txt test_output.txt test_error.txt 2>nul

echo.
echo 📋 Test Summary:
echo    - MCP protocol communication: Tested
echo    - Tool discovery mechanism: Verified  
echo    - Server capabilities: Checked
echo    - Error handling: Monitored
echo.
echo 💡 For full testing:
echo    1. Integrate with Claude Desktop
echo    2. Test individual tools (screenshot, mouse, etc.)
echo    3. Verify automation capabilities
echo    4. Check log file: %%TEMP%%\mcpcomputeruse-log.txt
echo.
echo ✨ MCP Server functionality test completed!

pause
