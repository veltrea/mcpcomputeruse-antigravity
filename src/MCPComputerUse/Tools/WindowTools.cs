using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;
using WindowsAutomation.WindowManagement;

namespace MCPComputerUse.Tools;

/// <summary>
/// Provides MCP tools for window management functionality.
/// </summary>
[McpServerToolType]
public static class WindowTools
{
    private static readonly JsonSerializerOptions DefaultJsonOptions = new()
    {
        WriteIndented = true
    };

    private static readonly WindowManager _windowManager = new();

    /// <summary>
    /// Lists all visible windows.
    /// </summary>
    /// <param name="includeHidden">Include hidden windows</param>
    /// <returns>A JSON string with the window list.</returns>
    [McpServerTool(Name = "list_windows")]
    [Description("List all visible windows")]
    public static string ListWindows(
        [Description("Include hidden windows")]
        bool includeHidden = false)
    {
        try
        {
            var windows = _windowManager.GetAllWindows(includeHidden);
            
            var windowList = windows.Select(w => new
            {
                id = (long)w.Handle,
                title = w.Title,
                processName = w.ProcessName,
                processId = w.ProcessId,
                x = w.X,
                y = w.Y,
                width = w.Width,
                height = w.Height,
                isVisible = w.IsVisible,
                isMinimized = w.IsMinimized,
                isMaximized = w.IsMaximized
            }).ToList();

            var result = new
            {
                success = true,
                count = windowList.Count,
                windows = windowList
            };

            return JsonSerializer.Serialize(result, DefaultJsonOptions);
        }
        catch (Exception ex)
        {
            var error = new
            {
                success = false,
                error = $"Failed to list windows: {ex.Message}"
            };
            return JsonSerializer.Serialize(error, DefaultJsonOptions);
        }
    }

    /// <summary>
    /// Gets currently active window info.
    /// </summary>
    /// <returns>A JSON string with the active window info.</returns>
    [McpServerTool(Name = "get_active_window")]
    [Description("Get currently active window info")]
    public static string GetActiveWindow()
    {
        try
        {
            var window = _windowManager.GetActiveWindow();
            if (window == null)
            {
                var noWindow = new
                {
                    success = true,
                    message = "No active window found",
                    window = (object?)null
                };
                return JsonSerializer.Serialize(noWindow, DefaultJsonOptions);
            }

            var result = new
            {
                success = true,
                window = new
                {
                    id = (long)window.Handle,
                    title = window.Title,
                    processName = window.ProcessName,
                    processId = window.ProcessId,
                    x = window.X,
                    y = window.Y,
                    width = window.Width,
                    height = window.Height,
                    isVisible = window.IsVisible,
                    isMinimized = window.IsMinimized,
                    isMaximized = window.IsMaximized
                }
            };

            return JsonSerializer.Serialize(result, DefaultJsonOptions);
        }
        catch (Exception ex)
        {
            var error = new
            {
                success = false,
                error = $"Failed to get active window: {ex.Message}"
            };
            return JsonSerializer.Serialize(error, DefaultJsonOptions);
        }
    }

    /// <summary>
    /// Focuses a window by ID or name.
    /// </summary>
    /// <param name="windowId">Window ID</param>
    /// <param name="windowName">Window title to search for</param>
    /// <returns>A JSON string with the focus result.</returns>
    [McpServerTool(Name = "focus_window")]
    [Description("Focus a window by ID or name")]
    public static string FocusWindow(
        [Description("Window ID")]
        long windowId = 0,
        [Description("Window title to search for")]
        string windowName = "")
    {
        try
        {
            bool success;
            string target;

            if (windowId != 0)
            {
                success = _windowManager.FocusWindow((IntPtr)windowId);
                target = $"window ID {windowId}";
            }
            else if (!string.IsNullOrEmpty(windowName))
            {
                success = _windowManager.FocusWindowByTitle(windowName);
                target = $"window '{windowName}'";
            }
            else
            {
                var error = new
                {
                    success = false,
                    error = "Either windowId or windowName must be specified"
                };
                return JsonSerializer.Serialize(error, DefaultJsonOptions);
            }

            var result = new
            {
                success,
                message = success ? $"Focused {target}" : $"Failed to focus {target}",
                target
            };

            return JsonSerializer.Serialize(result, DefaultJsonOptions);
        }
        catch (Exception ex)
        {
            var error = new
            {
                success = false,
                error = $"Failed to focus window: {ex.Message}"
            };
            return JsonSerializer.Serialize(error, DefaultJsonOptions);
        }
    }
}
