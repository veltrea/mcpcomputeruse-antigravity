using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MCPComputerUse;

// MCP Protocol Classes
public class MCPRequest
{
    public string jsonrpc { get; set; } = "2.0";
    public object? id { get; set; }
    public string method { get; set; } = "";
    public Dictionary<string, object>? @params { get; set; }
}

public class MCPResponse
{
    public string jsonrpc { get; set; } = "2.0";
    public object? id { get; set; }
    public object? result { get; set; }
    public MCPError? error { get; set; }
}

public class MCPError
{
    public int code { get; set; }
    public string message { get; set; } = "";
}

// Windows API Imports
public static class User32
{
    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    public const uint MOUSEEVENTF_LEFTUP = 0x0004;
    public const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
    public const uint MOUSEEVENTF_RIGHTUP = 0x0010;
}

public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    public int Left, Top, Right, Bottom;
}

// Main MCP Server
public class MCPComputerUseServer
{
    private readonly Dictionary<string, Func<MCPRequest, Task<MCPResponse>>> _tools;

    public MCPComputerUseServer()
    {
        _tools = new Dictionary<string, Func<MCPRequest, Task<MCPResponse>>>
        {
            ["computer-use:take_screenshot"] = TakeScreenshot,
            ["computer-use:mouse_click"] = MouseClick,
            ["computer-use:list_windows"] = ListWindows,
            ["computer-use:get_server_capabilities"] = GetServerCapabilities
        };
    }

    public async Task StartAsync()
    {
        Console.Error.WriteLine("MCPComputerUse Server Starting...");
        
        while (true)
        {
            try
            {
                var line = await Console.In.ReadLineAsync();
                if (line == null) break;

                if (string.IsNullOrWhiteSpace(line)) continue;

                var request = JsonSerializer.Deserialize<MCPRequest>(line);
                if (request == null) continue;

                var response = await HandleRequest(request);
                response.id = request.id;

                var responseJson = JsonSerializer.Serialize(response);
                await Console.Out.WriteLineAsync(responseJson);
                await Console.Out.FlushAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private async Task<MCPResponse> HandleRequest(MCPRequest request)
    {
        return request.method switch
        {
            "initialize" => HandleInitialize(),
            "tools/list" => HandleToolsList(),
            "tools/call" => await HandleToolCall(request),
            "notifications/initialized" => new MCPResponse { result = new { } },
            _ => new MCPResponse 
            { 
                error = new MCPError { code = -32601, message = $"Method not found: {request.method}" } 
            }
        };
    }

    private MCPResponse HandleInitialize()
    {
        return new MCPResponse
        {
            result = new
            {
                protocolVersion = "2024-11-05",
                capabilities = new { tools = new { } },
                serverInfo = new { name = "MCPComputerUse", version = "1.0.0" }
            }
        };
    }

    private MCPResponse HandleToolsList()
    {
        var tools = _tools.Keys.Select(name => new
        {
            name,
            description = GetToolDescription(name),
            inputSchema = GetToolSchema(name)
        }).ToArray();

        return new MCPResponse { result = new { tools } };
    }

    private async Task<MCPResponse> HandleToolCall(MCPRequest request)
    {
        try
        {
            var toolName = request.@params?["name"]?.ToString();
            if (string.IsNullOrEmpty(toolName) || !_tools.ContainsKey(toolName))
            {
                return new MCPResponse 
                { 
                    error = new MCPError { code = -32602, message = "Tool not found" } 
                };
            }

            return await _tools[toolName](request);
        }
        catch (Exception ex)
        {
            return new MCPResponse 
            { 
                error = new MCPError { code = -32603, message = ex.Message } 
            };
        }
    }

    private string GetToolDescription(string toolName)
    {
        return toolName switch
        {
            "computer-use:take_screenshot" => "Take a screenshot",
            "computer-use:mouse_click" => "Click the mouse at coordinates",
            "computer-use:list_windows" => "List all visible windows",
            "computer-use:get_server_capabilities" => "Get server capabilities",
            _ => "Unknown tool"
        };
    }

    private object GetToolSchema(string toolName)
    {
        return toolName switch
        {
            "computer-use:take_screenshot" => new
            {
                type = "object",
                properties = new
                {
                    filename = new { type = "string", description = "Optional filename" }
                }
            },
            "computer-use:mouse_click" => new
            {
                type = "object",
                properties = new
                {
                    x = new { type = "number", description = "X coordinate" },
                    y = new { type = "number", description = "Y coordinate" },
                    button = new { type = "string", description = "Mouse button (left/right)" }
                },
                required = new[] { "x", "y" }
            },
            _ => new { type = "object", properties = new { } }
        };
    }

    private async Task<MCPResponse> TakeScreenshot(MCPRequest request)
    {
        try
        {
            var args = request.@params?["arguments"] as JsonElement? ?? new JsonElement();
            var filename = "screenshot.png";
            
            if (args.ValueKind == JsonValueKind.Object && args.TryGetProperty("filename", out var filenameElement))
            {
                filename = filenameElement.GetString() ?? "screenshot.png";
            }

            if (!filename.EndsWith(".png")) filename += ".png";

            var screen = Screen.PrimaryScreen;
            using var bitmap = new Bitmap(screen.Bounds.Width, screen.Bounds.Height);
            using var graphics = Graphics.FromImage(bitmap);
            
            graphics.CopyFromScreen(0, 0, 0, 0, screen.Bounds.Size);
            bitmap.Save(filename, ImageFormat.Png);

            return new MCPResponse
            {
                result = new
                {
                    content = new[]
                    {
                        new { type = "text", text = $"Screenshot saved as {filename}" }
                    }
                }
            };
        }
        catch (Exception ex)
        {
            return new MCPResponse 
            { 
                error = new MCPError { code = -32603, message = $"Screenshot failed: {ex.Message}" } 
            };
        }
    }

    private async Task<MCPResponse> MouseClick(MCPRequest request)
    {
        try
        {
            var args = request.@params?["arguments"] as JsonElement? ?? new JsonElement();
            
            if (!args.TryGetProperty("x", out var xElement) || !args.TryGetProperty("y", out var yElement))
            {
                return new MCPResponse 
                { 
                    error = new MCPError { code = -32602, message = "x and y coordinates required" } 
                };
            }

            var x = xElement.GetInt32();
            var y = yElement.GetInt32();
            var button = "left";

            if (args.TryGetProperty("button", out var buttonElement))
            {
                button = buttonElement.GetString() ?? "left";
            }

            User32.SetCursorPos(x, y);
            System.Threading.Thread.Sleep(10);

            if (button == "left")
            {
                User32.mouse_event(User32.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
                User32.mouse_event(User32.MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
            }
            else if (button == "right")
            {
                User32.mouse_event(User32.MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
                User32.mouse_event(User32.MOUSEEVENTF_RIGHTUP, 0, 0, 0, UIntPtr.Zero);
            }

            return new MCPResponse
            {
                result = new
                {
                    content = new[]
                    {
                        new { type = "text", text = $"Clicked {button} button at ({x}, {y})" }
                    }
                }
            };
        }
        catch (Exception ex)
        {
            return new MCPResponse 
            { 
                error = new MCPError { code = -32603, message = $"Mouse click failed: {ex.Message}" } 
            };
        }
    }

    private async Task<MCPResponse> ListWindows(MCPRequest request)
    {
        try
        {
            var windows = new List<object>();

            User32.EnumWindows((hWnd, lParam) =>
            {
                if (User32.IsWindowVisible(hWnd))
                {
                    var title = new StringBuilder(256);
                    User32.GetWindowText(hWnd, title, title.Capacity);
                    
                    if (title.Length > 0)
                    {
                        User32.GetWindowRect(hWnd, out var rect);
                        windows.Add(new
                        {
                            id = (long)hWnd,
                            title = title.ToString(),
                            x = rect.Left,
                            y = rect.Top,
                            width = rect.Right - rect.Left,
                            height = rect.Bottom - rect.Top
                        });
                    }
                }
                return true;
            }, IntPtr.Zero);

            return new MCPResponse
            {
                result = new
                {
                    content = new[]
                    {
                        new { type = "text", text = $"Found {windows.Count} windows:\n" + 
                             string.Join("\n", windows.Take(10).Select(w => $"- {((dynamic)w).title}")) }
                    }
                }
            };
        }
        catch (Exception ex)
        {
            return new MCPResponse 
            { 
                error = new MCPError { code = -32603, message = $"List windows failed: {ex.Message}" } 
            };
        }
    }

    private async Task<MCPResponse> GetServerCapabilities(MCPRequest request)
    {
        var capabilities = new
        {
            server_name = "MCPComputerUse",
            version = "1.0.0",
            platform = "Windows",
            capabilities = new[] { "screenshot_capture", "mouse_automation", "window_management" },
            tools = _tools.Keys.ToArray()
        };

        return new MCPResponse
        {
            result = new
            {
                content = new[]
                {
                    new { type = "text", text = JsonSerializer.Serialize(capabilities, new JsonSerializerOptions { WriteIndented = true }) }
                }
            }
        };
    }
}

// Program Entry Point
public class Program
{
    [STAThread]
    public static async Task Main(string[] args)
    {
        try
        {
            var server = new MCPComputerUseServer();
            await server.StartAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Fatal error: {ex.Message}");
            Environment.Exit(1);
        }
    }
}
