using System.Diagnostics;
using WindowsAutomation.Input;
using WindowsAutomation.ScreenCapture;
using WindowsAutomation.WindowManagement;
using WindowsAutomation.Native;

namespace WindowsAutomation.Macros;

public class MacroEngine
{
    private readonly MouseController _mouse;
    private readonly KeyboardController _keyboard;
    private readonly ScreenshotService _screenshot;
    private readonly WindowManager _windowManager;
    private readonly WindowCapture _windowCapture;

    public MacroEngine()
    {
        _mouse = new MouseController();
        _keyboard = new KeyboardController();
        _screenshot = new ScreenshotService();
        _windowManager = new WindowManager();
        _windowCapture = new WindowCapture();
    }

    public async Task<MacroResult> ExecuteAsync(MacroCommand[] commands, string? name = null)
    {
        var stopwatch = Stopwatch.StartNew();
        var results = new List<string>();
        var macroResult = new MacroResult();

        try
        {
            foreach (var (command, index) in commands.Select((c, i) => (c, i)))
            {
                try
                {
                    var result = await ExecuteCommandAsync(command);
                    results.Add($"{index + 1}. {result}");
                    
                    // Default delay between commands
                    if (index < commands.Length - 1)
                        await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    var errorMsg = $"{index + 1}. Error: {ex.Message}";
                    results.Add(errorMsg);
                    
                    macroResult.Success = false;
                    macroResult.ErrorMessage = ex.Message;
                    break; // Stop execution on error
                }
            }
        }
        catch (Exception ex)
        {
            macroResult.Success = false;
            macroResult.ErrorMessage = ex.Message;
            results.Add($"Macro execution failed: {ex.Message}");
        }
        finally
        {
            stopwatch.Stop();
            macroResult.ExecutionTime = stopwatch.Elapsed;
            macroResult.Results = results;
        }

        return macroResult;
    }

    private async Task<string> ExecuteCommandAsync(MacroCommand command)
    {
        return command.Action.ToLower() switch
        {
            "click" => ExecuteClick(command),
            "move" => ExecuteMove(command),
            "type" => ExecuteType(command),
            "key" => ExecuteKey(command),
            "scroll" => ExecuteScroll(command),
            "wait" => await ExecuteWaitAsync(command),
            "screenshot" => await ExecuteScreenshotAsync(command),
            "focus_window" => ExecuteFocusWindow(command),
            _ => throw new ArgumentException($"Unknown command action: {command.Action}")
        };
    }

    private string ExecuteClick(MacroCommand command)
    {
        var button = ParseMouseButton(command.Button ?? "left");
        _mouse.ClickAt(command.X, command.Y, button, command.Clicks);
        return $"Clicked {button} button at ({command.X}, {command.Y}) {command.Clicks} time(s)";
    }

    private string ExecuteMove(MacroCommand command)
    {
        _mouse.MoveTo(command.X, command.Y);
        return $"Moved mouse to ({command.X}, {command.Y})";
    }

    private string ExecuteType(MacroCommand command)
    {
        if (string.IsNullOrEmpty(command.Text))
            throw new ArgumentException("Text is required for type command");
            
        _keyboard.TypeText(command.Text);
        return $"Typed text: '{command.Text}'";
    }

    private string ExecuteKey(MacroCommand command)
    {
        if (string.IsNullOrEmpty(command.Key))
            throw new ArgumentException("Key is required for key command");

        var modifiers = command.Modifiers?.Select(ParseModifier).ToArray() ?? Array.Empty<VirtualKeyCode>();
        var key = ParseKey(command.Key);
        
        _keyboard.PressKey(key, modifiers);
        
        var modifierText = modifiers.Length > 0 ? string.Join("+", modifiers) + "+" : "";
        return $"Pressed key: {modifierText}{key}";
    }

    private string ExecuteScroll(MacroCommand command)
    {
        var direction = command.ScrollDirection?.ToLower() ?? "down";
        var delta = direction == "up" ? 3 : -3;
        
        _mouse.ScrollVertical(delta, command.X, command.Y);
        return $"Scrolled {direction} at ({command.X}, {command.Y})";
    }

    private async Task<string> ExecuteWaitAsync(MacroCommand command)
    {
        await Task.Delay(command.Duration);
        return $"Waited {command.Duration} ms";
    }

    private async Task<string> ExecuteScreenshotAsync(MacroCommand command)
    {
        var filename = command.Filename ?? $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
        if (!filename.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            filename += ".png";

        byte[] imageData;
        if (command.WindowId != 0)
        {
            imageData = _windowCapture.CaptureWindow((IntPtr)command.WindowId);
        }
        else if (!string.IsNullOrEmpty(command.WindowName))
        {
            imageData = _windowCapture.CaptureWindowByTitle(command.WindowName);
        }
        else
        {
            imageData = _screenshot.CaptureScreen();
        }

        await File.WriteAllBytesAsync(filename, imageData);
        return $"Screenshot saved: {filename}";
    }

    private string ExecuteFocusWindow(MacroCommand command)
    {
        bool success;
        string target;
        
        if (command.WindowId != 0)
        {
            success = _windowManager.FocusWindow((IntPtr)command.WindowId);
            target = $"window ID {command.WindowId}";
        }
        else if (!string.IsNullOrEmpty(command.WindowName))
        {
            success = _windowManager.FocusWindowByTitle(command.WindowName);
            target = $"window '{command.WindowName}'";
        }
        else
        {
            throw new ArgumentException("Either WindowId or WindowName must be specified for focus_window command");
        }

        return success ? $"Focused {target}" : $"Failed to focus {target}";
    }

    private static MouseButton ParseMouseButton(string button)
    {
        return button.ToLower() switch
        {
            "left" => MouseButton.Left,
            "right" => MouseButton.Right,
            "middle" => MouseButton.Middle,
            _ => throw new ArgumentException($"Unknown mouse button: {button}")
        };
    }

    private static VirtualKeyCode ParseModifier(string modifier)
    {
        return modifier.ToLower() switch
        {
            "ctrl" or "control" => VirtualKeyCode.VK_CONTROL,
            "alt" => VirtualKeyCode.VK_MENU,
            "shift" => VirtualKeyCode.VK_SHIFT,
            "cmd" or "win" or "windows" => VirtualKeyCode.VK_LWIN,
            _ => throw new ArgumentException($"Unknown modifier: {modifier}")
        };
    }

    private static VirtualKeyCode ParseKey(string key)
    {
        return key.ToLower() switch
        {
            "enter" or "return" => VirtualKeyCode.VK_RETURN,
            "space" => VirtualKeyCode.VK_SPACE,
            "tab" => VirtualKeyCode.VK_TAB,
            "escape" or "esc" => VirtualKeyCode.VK_ESCAPE,
            "backspace" => VirtualKeyCode.VK_BACK,
            "delete" or "del" => VirtualKeyCode.VK_DELETE,
            "insert" => VirtualKeyCode.VK_INSERT,
            "home" => VirtualKeyCode.VK_HOME,
            "end" => VirtualKeyCode.VK_END,
            "pageup" => VirtualKeyCode.VK_PRIOR,
            "pagedown" => VirtualKeyCode.VK_NEXT,
            "up" => VirtualKeyCode.VK_UP,
            "down" => VirtualKeyCode.VK_DOWN,
            "left" => VirtualKeyCode.VK_LEFT,
            "right" => VirtualKeyCode.VK_RIGHT,
            "f1" => VirtualKeyCode.VK_F1,
            "f2" => VirtualKeyCode.VK_F2,
            "f3" => VirtualKeyCode.VK_F3,
            "f4" => VirtualKeyCode.VK_F4,
            "f5" => VirtualKeyCode.VK_F5,
            "f6" => VirtualKeyCode.VK_F6,
            "f7" => VirtualKeyCode.VK_F7,
            "f8" => VirtualKeyCode.VK_F8,
            "f9" => VirtualKeyCode.VK_F9,
            "f10" => VirtualKeyCode.VK_F10,
            "f11" => VirtualKeyCode.VK_F11,
            "f12" => VirtualKeyCode.VK_F12,
            _ when key.Length == 1 && char.IsLetter(key[0]) => 
                (VirtualKeyCode)((int)VirtualKeyCode.VK_A + (key.ToUpper()[0] - 'A')),
            _ when key.Length == 1 && char.IsDigit(key[0]) => 
                (VirtualKeyCode)((int)VirtualKeyCode.VK_0 + (key[0] - '0')),
            _ => throw new ArgumentException($"Unknown key: {key}")
        };
    }
}
