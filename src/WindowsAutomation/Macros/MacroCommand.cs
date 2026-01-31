using WindowsAutomation.Native;

namespace WindowsAutomation.Macros;

public class MacroCommand
{
    public string Action { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
    public string? Text { get; set; }
    public string? Key { get; set; }
    public string[]? Modifiers { get; set; }
    public string? Button { get; set; }
    public int Clicks { get; set; } = 1;
    public int Duration { get; set; }
    public string? ScrollDirection { get; set; }
    public string? Filename { get; set; }
    public int WindowId { get; set; }
    public string? WindowName { get; set; }
}

public class MacroResult
{
    public List<string> Results { get; set; } = new();
    public bool Success { get; set; } = true;
    public string? ErrorMessage { get; set; }
    public TimeSpan ExecutionTime { get; set; }
}
