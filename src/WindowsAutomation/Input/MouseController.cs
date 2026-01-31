using WindowsAutomation.Native;

namespace WindowsAutomation.Input;

public class MouseController
{
    public void MoveTo(int x, int y)
    {
        User32.SetCursorPos(x, y);
    }

    public POINT GetPosition()
    {
        User32.GetCursorPos(out var point);
        return point;
    }

    public void Click(MouseButton button = MouseButton.Left, int clicks = 1)
    {
        var (downFlag, upFlag) = GetMouseFlags(button);

        for (int i = 0; i < clicks; i++)
        {
            User32.mouse_event(downFlag, 0, 0, 0, UIntPtr.Zero);
            Thread.Sleep(10); // Small delay between down and up
            User32.mouse_event(upFlag, 0, 0, 0, UIntPtr.Zero);
            
            if (i < clicks - 1)
                Thread.Sleep(50); // Delay between multiple clicks
        }
    }

    public void ClickAt(int x, int y, MouseButton button = MouseButton.Left, int clicks = 1)
    {
        MoveTo(x, y);
        Thread.Sleep(10); // Small delay to ensure cursor position is set
        Click(button, clicks);
    }

    public void Drag(int startX, int startY, int endX, int endY, MouseButton button = MouseButton.Left)
    {
        MoveTo(startX, startY);
        Thread.Sleep(10);

        var (downFlag, upFlag) = GetMouseFlags(button);
        
        // Press down
        User32.mouse_event(downFlag, 0, 0, 0, UIntPtr.Zero);
        Thread.Sleep(50);

        // Move to end position
        MoveTo(endX, endY);
        Thread.Sleep(50);

        // Release
        User32.mouse_event(upFlag, 0, 0, 0, UIntPtr.Zero);
    }

    public void Scroll(int x, int y, int delta)
    {
        MoveTo(x, y);
        Thread.Sleep(10);
        User32.mouse_event(User32.MOUSEEVENTF_WHEEL, 0, 0, (uint)delta, UIntPtr.Zero);
    }

    public void ScrollVertical(int delta, int x = -1, int y = -1)
    {
        if (x == -1 || y == -1)
        {
            var pos = GetPosition();
            x = x == -1 ? pos.X : x;
            y = y == -1 ? pos.Y : y;
        }

        Scroll(x, y, delta * 120); // 120 is the standard wheel delta
    }

    public void ScrollHorizontal(int delta, int x = -1, int y = -1)
    {
        if (x == -1 || y == -1)
        {
            var pos = GetPosition();
            x = x == -1 ? pos.X : x;
            y = y == -1 ? pos.Y : y;
        }

        MoveTo(x, y);
        Thread.Sleep(10);
        User32.mouse_event(0x01000, 0, 0, (uint)(delta * 120), UIntPtr.Zero); // MOUSEEVENTF_HWHEEL
    }

    private static (uint downFlag, uint upFlag) GetMouseFlags(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => (User32.MOUSEEVENTF_LEFTDOWN, User32.MOUSEEVENTF_LEFTUP),
            MouseButton.Right => (User32.MOUSEEVENTF_RIGHTDOWN, User32.MOUSEEVENTF_RIGHTUP),
            MouseButton.Middle => (User32.MOUSEEVENTF_MIDDLEDOWN, User32.MOUSEEVENTF_MIDDLEUP),
            _ => throw new ArgumentException($"Unsupported mouse button: {button}")
        };
    }
}
