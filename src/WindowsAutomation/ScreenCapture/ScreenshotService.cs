using System.Drawing;
using System.Drawing.Imaging;
using WindowsAutomation.Native;

namespace WindowsAutomation.ScreenCapture;

public class ScreenshotService
{
    public byte[] CaptureScreen(int displayIndex = 0)
    {
        var screens = Screen.AllScreens;
        if (displayIndex >= screens.Length)
            throw new ArgumentException($"Display index {displayIndex} is out of range. Available displays: {screens.Length}");

        var screen = screens[displayIndex];
        return CaptureRegion(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height);
    }

    public byte[] CaptureRegion(int x, int y, int width, int height)
    {
        using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(bitmap);
        
        graphics.CopyFromScreen(x, y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
        
        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        return stream.ToArray();
    }

    public List<DisplayInfo> GetDisplays()
    {
        var displays = new List<DisplayInfo>();
        var index = 0;

        // Refactor: use a local function instead of lambda with 'ref'
        bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData)
        {
            var info = new MONITORINFO();
            if (User32.GetMonitorInfo(hMonitor, ref info))
            {
                displays.Add(new DisplayInfo
                {
                    Index = index++,
                    Bounds = new Rectangle(info.rcMonitor.Left, info.rcMonitor.Top, info.rcMonitor.Width, info.rcMonitor.Height),
                    WorkingArea = new Rectangle(info.rcWork.Left, info.rcWork.Top, info.rcWork.Width, info.rcWork.Height),
                    IsPrimary = (info.dwFlags & 1) != 0
                });
            }
            return true;
        }

        User32.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnumProc, IntPtr.Zero);
        return displays;
    }
}

public class DisplayInfo
{
    public int Index { get; set; }
    public Rectangle Bounds { get; set; }
    public Rectangle WorkingArea { get; set; }
    public bool IsPrimary { get; set; }
    public int Width => Bounds.Width;
    public int Height => Bounds.Height;
    public int X => Bounds.X;
    public int Y => Bounds.Y;
}
