using WindowsAutomation.Native;

namespace WindowsAutomation.Input;

public class KeyboardController
{
    public void TypeText(string text)
    {
        foreach (char c in text)
        {
            if (c == '\n')
            {
                PressKey(VirtualKeyCode.VK_RETURN);
            }
            else if (c == '\t')
            {
                PressKey(VirtualKeyCode.VK_TAB);
            }
            else
            {
                SendChar(c);
            }
            
            Thread.Sleep(10); // Small delay between characters
        }
    }

    public void PressKey(VirtualKeyCode key, params VirtualKeyCode[] modifiers)
    {
        // Press modifiers
        foreach (var mod in modifiers)
        {
            User32.keybd_event((byte)mod, 0, 0, UIntPtr.Zero);
            Thread.Sleep(10);
        }

        // Press main key
        User32.keybd_event((byte)key, 0, 0, UIntPtr.Zero);
        Thread.Sleep(50);
        User32.keybd_event((byte)key, 0, User32.KEYEVENTF_KEYUP, UIntPtr.Zero);

        // Release modifiers in reverse order
        foreach (var mod in modifiers.Reverse())
        {
            Thread.Sleep(10);
            User32.keybd_event((byte)mod, 0, User32.KEYEVENTF_KEYUP, UIntPtr.Zero);
        }
    }

    public void PressKeys(params VirtualKeyCode[] keys)
    {
        foreach (var key in keys)
        {
            User32.keybd_event((byte)key, 0, 0, UIntPtr.Zero);
            Thread.Sleep(10);
        }

        Thread.Sleep(50);

        foreach (var key in keys.Reverse())
        {
            User32.keybd_event((byte)key, 0, User32.KEYEVENTF_KEYUP, UIntPtr.Zero);
            Thread.Sleep(10);
        }
    }

    public void HoldKey(VirtualKeyCode key)
    {
        User32.keybd_event((byte)key, 0, 0, UIntPtr.Zero);
    }

    public void ReleaseKey(VirtualKeyCode key)
    {
        User32.keybd_event((byte)key, 0, User32.KEYEVENTF_KEYUP, UIntPtr.Zero);
    }

    public bool IsKeyPressed(VirtualKeyCode key)
    {
        return (User32.GetKeyState((int)key) & 0x8000) != 0;
    }

    public void SendKeyCombo(string combo)
    {
        var parts = combo.ToLower().Split('+');
        var modifiers = new List<VirtualKeyCode>();
        VirtualKeyCode mainKey = 0;

        foreach (var part in parts)
        {
            var trimmed = part.Trim();
            switch (trimmed)
            {
                case "ctrl":
                case "control":
                    modifiers.Add(VirtualKeyCode.VK_CONTROL);
                    break;
                case "alt":
                    modifiers.Add(VirtualKeyCode.VK_MENU);
                    break;
                case "shift":
                    modifiers.Add(VirtualKeyCode.VK_SHIFT);
                    break;
                case "win":
                case "windows":
                    modifiers.Add(VirtualKeyCode.VK_LWIN);
                    break;
                default:
                    mainKey = ParseKeyName(trimmed);
                    break;
            }
        }

        if (mainKey != 0)
        {
            PressKey(mainKey, modifiers.ToArray());
        }
    }

    private void SendChar(char c)
    {
        // For Unicode characters, use keybd_event with KEYEVENTF_UNICODE
        User32.keybd_event(0, 0, User32.KEYEVENTF_UNICODE, (UIntPtr)c);
        User32.keybd_event(0, 0, User32.KEYEVENTF_UNICODE | User32.KEYEVENTF_KEYUP, (UIntPtr)c);
    }

    private VirtualKeyCode ParseKeyName(string keyName)
    {
        return keyName.ToLower() switch
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
            _ when keyName.Length == 1 && char.IsLetter(keyName[0]) => 
                (VirtualKeyCode)((int)VirtualKeyCode.VK_A + (keyName.ToUpper()[0] - 'A')),
            _ when keyName.Length == 1 && char.IsDigit(keyName[0]) => 
                (VirtualKeyCode)((int)VirtualKeyCode.VK_0 + (keyName[0] - '0')),
            _ => throw new ArgumentException($"Unknown key name: {keyName}")
        };
    }
}
