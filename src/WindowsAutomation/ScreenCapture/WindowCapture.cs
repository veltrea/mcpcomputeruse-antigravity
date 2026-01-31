using System.Drawing;
using System.Drawing.Imaging;
using WindowsAutomation.Native;

namespace WindowsAutomation.ScreenCapture;

public class WindowCapture
{
    public byte[] CaptureWindow(IntPtr hWnd)
    {
        if (!User32.GetWindowRect(hWnd, out var rect))
            throw new InvalidOperationException("Unable to get window rectangle");

        var width = rect.Width;
        var height = rect.Height;

        if (width <= 0 || height <= 0)
            throw new InvalidOperationException("Window has invalid dimensions");

        using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(bitmap);
        
        var hdc = graphics.GetHdc();
        try
        {
            User32.PrintWindow(hWnd, hdc, 0);
        }
        finally
        {
            graphics.ReleaseHdc(hdc);
        }

        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        return stream.ToArray();
    }

    public byte[] CaptureActiveWindow()
    {
        var hWnd = User32.GetForegroundWindow();
        if (hWnd == IntPtr.Zero)
            throw new InvalidOperationException("No active window found");

        return CaptureWindow(hWnd);
    }

    public byte[] CaptureWindowByTitle(string title, bool exactMatch = false)
    {
        var hWnd = FindWindowByTitle(title, exactMatch);
        if (hWnd == IntPtr.Zero)
            throw new InvalidOperationException($"Window with title '{title}' not found");

        return CaptureWindow(hWnd);
    }

    private IntPtr FindWindowByTitle(string title, bool exactMatch)
    {
        IntPtr foundWindow = IntPtr.Zero;

        User32.EnumWindows((hWnd, lParam) =>
        {
            var windowTitle = GetWindowTitle(hWnd);
            bool matches = exactMatch ? 
                string.Equals(windowTitle, title, StringComparison.OrdinalIgnoreCase) :
                windowTitle.Contains(title, StringComparison.OrdinalIgnoreCase);

            if (matches && User32.IsWindowVisible(hWnd))
            {
                foundWindow = hWnd;
                return false; // Stop enumeration
            }
            return true; // Continue enumeration
        }, IntPtr.Zero);

        return foundWindow;
    }

    private string GetWindowTitle(IntPtr hWnd)
    {
        var length = User32.GetWindowTextLength(hWnd);
        if (length == 0) return string.Empty;

        var title = new System.Text.StringBuilder(length + 1);
        User32.GetWindowText(hWnd, title, title.Capacity);
        return title.ToString();
    }
}
