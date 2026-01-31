using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;
using WindowsAutomation.Macros;

namespace MCPComputerUse.Tools;

/// <summary>
/// Provides MCP tools for macro execution and server capabilities.
/// </summary>
[McpServerToolType]
public static class MacroAndUtilityTools
{
    private static readonly JsonSerializerOptions DefaultJsonOptions = new()
    {
        WriteIndented = true
    };

    private static readonly MacroEngine _macroEngine = new();

    /// <summary>
    /// Executes a sequence of automation commands.
    /// </summary>
    /// <param name="commandsJson">JSON array of commands to execute</param>
    /// <param name="name">Optional macro name</param>
    /// <returns>A JSON string with the macro execution result.</returns>
    [McpServerTool(Name = "run_macro")]
    [Description("Execute a sequence of automation commands")]
    public static string RunMacro(
        [Description("JSON array of commands to execute")] string commandsJson,
        [Description("Optional macro name")] string name = "")
    {
        try
        {
            var commandArray = JsonSerializer.Deserialize<JsonElement[]>(commandsJson);
            if (commandArray == null)
            {
                var error = new
                {
                    success = false,
                    error = "Invalid commands JSON format"
                };
                return JsonSerializer.Serialize(error, DefaultJsonOptions);
            }

            var commands = new List<MacroCommand>();
            foreach (var cmdElement in commandArray)
            {
                var command = new MacroCommand
                {
                    Action = cmdElement.TryGetProperty("action", out var actionProp) ? actionProp.GetString() ?? "" : "",
                    X = cmdElement.TryGetProperty("x", out var xProp) ? xProp.GetInt32() : 0,
                    Y = cmdElement.TryGetProperty("y", out var yProp) ? yProp.GetInt32() : 0,
                    Text = cmdElement.TryGetProperty("text", out var textProp) ? textProp.GetString() : null,
                    Key = cmdElement.TryGetProperty("key", out var keyProp) ? keyProp.GetString() : null,
                    Button = cmdElement.TryGetProperty("button", out var buttonProp) ? buttonProp.GetString() : null,
                    Clicks = cmdElement.TryGetProperty("clicks", out var clicksProp) ? clicksProp.GetInt32() : 1,
                    Duration = cmdElement.TryGetProperty("duration", out var durationProp) ? durationProp.GetInt32() : 0,
                    ScrollDirection = cmdElement.TryGetProperty("scrollDirection", out var scrollProp) ? scrollProp.GetString() : null,
                    Filename = cmdElement.TryGetProperty("filename", out var filenameProp) ? filenameProp.GetString() : null,
                    WindowId = cmdElement.TryGetProperty("window_id", out var windowIdProp) ? windowIdProp.GetInt32() : 0,
                    WindowName = cmdElement.TryGetProperty("window_name", out var windowNameProp) ? windowNameProp.GetString() : null
                };

                if (cmdElement.TryGetProperty("modifiers", out var modifiersProp) && modifiersProp.ValueKind == JsonValueKind.Array)
                {
                    command.Modifiers = modifiersProp.EnumerateArray().Select(m => m.GetString() ?? "").ToArray();
                }

                commands.Add(command);
            }

            var task = Task.Run(async () => await _macroEngine.ExecuteAsync(commands.ToArray(), name));
            var result = task.GetAwaiter().GetResult();

            var response = new
            {
                success = result.Success,
                message = $"Macro execution {(result.Success ? "completed" : "failed")} in {result.ExecutionTime.TotalMilliseconds:F0}ms",
                executionTime = result.ExecutionTime.TotalMilliseconds,
                commandCount = commands.Count,
                results = result.Results,
                errorMessage = result.ErrorMessage
            };

            return JsonSerializer.Serialize(response, DefaultJsonOptions);
        }
        catch (Exception ex)
        {
            var error = new
            {
                success = false,
                error = $"Macro execution failed: {ex.Message}",
                name
            };
            return JsonSerializer.Serialize(error, DefaultJsonOptions);
        }
    }

    /// <summary>
    /// Gets server capabilities and status.
    /// </summary>
    /// <returns>A JSON string with server capabilities.</returns>
    [McpServerTool(Name = "get_server_capabilities")]
    [Description("Get server capabilities and status")]
    public static string GetServerCapabilities()
    {
        try
        {
            var capabilities = new
            {
                serverName = "MCPComputerUse",
                version = "1.0.0",
                platform = "Windows",
                sdkVersion = "Official ModelContextProtocol SDK",
                capabilities = new[]
                {
                    "screenshot_capture",
                    "window_management", 
                    "mouse_automation",
                    "keyboard_automation",
                    "macro_execution",
                    "multi_monitor_support"
                },
                tools = new[]
                {
                    "take_screenshot",
                    "list_windows",
                    "get_active_window", 
                    "focus_window",
                    "mouse_click",
                    "mouse_move",
                    "type_text",
                    "press_key",
                    "scroll",
                    "run_macro",
                    "get_server_capabilities"
                },
                features = new
                {
                    multiMonitorSupport = true,
                    windowManagement = true,
                    macroExecution = true,
                    unicodeTextInput = true,
                    nativeWindowsApi = true,
                    selfContainedDeployment = true
                },
                systemInfo = new
                {
                    operatingSystem = Environment.OSVersion.ToString(),
                    processorCount = Environment.ProcessorCount,
                    workingSet = Environment.WorkingSet,
                    clrVersion = Environment.Version.ToString()
                }
            };

            var result = new
            {
                success = true,
                capabilities
            };

            return JsonSerializer.Serialize(result, DefaultJsonOptions);
        }
        catch (Exception ex)
        {
            var error = new
            {
                success = false,
                error = $"Failed to get server capabilities: {ex.Message}"
            };
            return JsonSerializer.Serialize(error, DefaultJsonOptions);
        }
    }

    /// <summary>
    /// Provides a space for structured thinking during complex operations.
    /// </summary>
    /// <param name="thought">The thought or reasoning to process</param>
    /// <returns>A JSON string acknowledging the thought.</returns>
    [McpServerTool(Name = "think")]
    [Description("Provides a space for structured thinking during complex operations")]
    public static string Think(
        [Description("The thought or reasoning to process")] string thought)
    {
        try
        {
            var result = new
            {
                success = true,
                message = "Thought processed",
                thought,
                timestamp = DateTime.Now,
                wordCount = thought.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length
            };

            return JsonSerializer.Serialize(result, DefaultJsonOptions);
        }
        catch (Exception ex)
        {
            var error = new
            {
                success = false,
                error = $"Think operation failed: {ex.Message}",
                thought
            };
            return JsonSerializer.Serialize(error, DefaultJsonOptions);
        }
    }
}
