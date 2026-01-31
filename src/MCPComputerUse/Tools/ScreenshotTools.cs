using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;
using WindowsAutomation.ScreenCapture;

namespace MCPComputerUse.Tools;

/// <summary>
/// Provides MCP tools for screenshot capture functionality.
/// </summary>
[McpServerToolType]
public static class ScreenshotTools
{
    private static readonly JsonSerializerOptions DefaultJsonOptions = new()
    {
        WriteIndented = true
    };

    private static readonly ScreenshotService _screenshotService = new();
    private static readonly WindowCapture _windowCapture = new();

    /// <summary>
    /// Takes a screenshot with flexible targeting options.
    /// </summary>
    /// <param name="filename">Optional filename (without extension)</param>
    /// <param name="screenId">Display number (0=primary)</param>
    /// <param name="target">Target: "screen", "active_window", "window"</param>
    /// <param name="windowId">Window ID to screenshot</param>
    /// <param name="windowName">Window title to search for</param>
    /// <returns>A JSON string with the screenshot result.</returns>
    [McpServerTool(Name = "take_screenshot")]
    [Description("Take a screenshot with flexible targeting options")]
    public static string TakeScreenshot(
        [Description("Optional filename (without extension)")] 
        string filename = "",
        [Description("Display number (0=primary)")] 
        int screenId = 0,
        [Description("Target: \"screen\", \"active_window\", \"window\"")] 
        string target = "screen",
        [Description("Window ID to screenshot")] 
        long windowId = 0,
        [Description("Window title to search for")] 
        string windowName = "")
    {
        try
        {
            if (string.IsNullOrEmpty(filename))
                filename = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}";

            if (!filename.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                filename += ".png";

            byte[] imageData = target.ToLower() switch
            {
                "active_window" => _windowCapture.CaptureActiveWindow(),
                "window" => CaptureSpecificWindow(windowId, windowName),
                _ => _screenshotService.CaptureScreen(screenId)
            };

            var filepath = Path.GetFullPath(filename);
            File.WriteAllBytes(filepath, imageData);

            var result = new
            {
                success = true,
                message = $"Screenshot saved: {filepath}",
                filepath,
                target,
                screenId,
                size = imageData.Length
            };

            return JsonSerializer.Serialize(result, DefaultJsonOptions);
        }
        catch (Exception ex)
        {
            var error = new
            {
                success = false,
                error = $"Screenshot failed: {ex.Message}",
                target
            };
            return JsonSerializer.Serialize(error, DefaultJsonOptions);
        }
    }

    private static byte[] CaptureSpecificWindow(long windowId, string windowName)
    {
        if (windowId != 0)
        {
            return _windowCapture.CaptureWindow((IntPtr)windowId);
        }

        if (!string.IsNullOrEmpty(windowName))
        {
            return _windowCapture.CaptureWindowByTitle(windowName);
        }

        throw new ArgumentException("Either windowId or windowName must be specified for window target");
    }
}
