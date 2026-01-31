using System.Drawing;
using WindowsAutomation.Native;

namespace WindowsAutomation.WindowManagement;

public class WindowInfo
{
    public IntPtr Handle { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public uint ProcessId { get; set; }
    public Rectangle Bounds { get; set; }
    public bool IsVisible { get; set; }
    public bool IsMinimized { get; set; }
    public bool IsMaximized { get; set; }
    public int Width => Bounds.Width;
    public int Height => Bounds.Height;
    public int X => Bounds.X;
    public int Y => Bounds.Y;
}

public class WindowManager
{
    public List<WindowInfo> GetAllWindows(bool includeHidden = false)
    {
        var windows = new List<WindowInfo>();

        User32.EnumWindows((hWnd, lParam) =>
        {
            var isVisible = User32.IsWindowVisible(hWnd);
            if (!includeHidden && !isVisible)
                return true;

            var title = GetWindowTitle(hWnd);
            if (string.IsNullOrEmpty(title) && !includeHidden)
                return true;

            User32.GetWindowThreadProcessId(hWnd, out uint processId);
            var processName = GetProcessName(processId);

            User32.GetWindowRect(hWnd, out var rect);

            var info = new WindowInfo
            {
                Handle = hWnd,
                Title = title,
                ProcessName = processName,
                ProcessId = processId,
                Bounds = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height),
                IsVisible = isVisible,
                IsMinimized = User32.IsIconic(hWnd),
                IsMaximized = IsWindowMaximized(hWnd)
            };

            windows.Add(info);
            return true;
        }, IntPtr.Zero);

        return windows;
    }

    public WindowInfo? GetActiveWindow()
    {
        var hWnd = User32.GetForegroundWindow();
        if (hWnd == IntPtr.Zero)
            return null;

        return GetWindowInfo(hWnd);
    }

    public WindowInfo? GetWindowInfo(IntPtr hWnd)
    {
        if (hWnd == IntPtr.Zero)
            return null;

        var title = GetWindowTitle(hWnd);
        User32.GetWindowThreadProcessId(hWnd, out uint processId);
        var processName = GetProcessName(processId);
        User32.GetWindowRect(hWnd, out var rect);

        return new WindowInfo
        {
            Handle = hWnd,
            Title = title,
            ProcessName = processName,
            ProcessId = processId,
            Bounds = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height),
            IsVisible = User32.IsWindowVisible(hWnd),
            IsMinimized = User32.IsIconic(hWnd),
            IsMaximized = IsWindowMaximized(hWnd)
        };
    }

    public bool FocusWindow(IntPtr hWnd)
    {
        if (hWnd == IntPtr.Zero)
            return false;

        // If window is minimized, restore it first
        if (User32.IsIconic(hWnd))
        {
            User32.ShowWindow(hWnd, User32.SW_RESTORE);
            Thread.Sleep(100);
        }

        return User32.SetForegroundWindow(hWnd);
    }

    public bool FocusWindowByTitle(string title, bool exactMatch = false)
    {
        var window = FindWindowByTitle(title, exactMatch);
        return window != null && FocusWindow(window.Handle);
    }

    public bool FocusWindowByProcessName(string processName)
    {
        var window = FindWindowByProcessName(processName);
        return window != null && FocusWindow(window.Handle);
    }

    public WindowInfo? FindWindowByTitle(string title, bool exactMatch = false)
    {
        var windows = GetAllWindows(false);
        return windows.FirstOrDefault(w => 
            exactMatch ? 
            string.Equals(w.Title, title, StringComparison.OrdinalIgnoreCase) :
            w.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
    }

    public WindowInfo? FindWindowByProcessName(string processName)
    {
        var windows = GetAllWindows(false);
        return windows.FirstOrDefault(w => 
            string.Equals(w.ProcessName, processName, StringComparison.OrdinalIgnoreCase));
    }

    public List<WindowInfo> FindWindowsByTitle(string title, bool exactMatch = false)
    {
        var windows = GetAllWindows(false);
        return windows.Where(w => 
            exactMatch ? 
            string.Equals(w.Title, title, StringComparison.OrdinalIgnoreCase) :
            w.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public List<WindowInfo> FindWindowsByProcessName(string processName)
    {
        var windows = GetAllWindows(false);
        return windows.Where(w => 
            string.Equals(w.ProcessName, processName, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public bool MinimizeWindow(IntPtr hWnd)
    {
        return User32.ShowWindow(hWnd, User32.SW_SHOWMINIMIZED);
    }

    public bool MaximizeWindow(IntPtr hWnd)
    {
        return User32.ShowWindow(hWnd, User32.SW_SHOWMAXIMIZED);
    }

    public bool RestoreWindow(IntPtr hWnd)
    {
        return User32.ShowWindow(hWnd, User32.SW_RESTORE);
    }

    public bool HideWindow(IntPtr hWnd)
    {
        return User32.ShowWindow(hWnd, User32.SW_HIDE);
    }

    public bool ShowWindow(IntPtr hWnd)
    {
        return User32.ShowWindow(hWnd, User32.SW_SHOWNORMAL);
    }

    private string GetWindowTitle(IntPtr hWnd)
    {
        var length = User32.GetWindowTextLength(hWnd);
        if (length == 0) return string.Empty;

        var title = new System.Text.StringBuilder(length + 1);
        User32.GetWindowText(hWnd, title, title.Capacity);
        return title.ToString();
    }

    private string GetProcessName(uint processId)
    {
        try
        {
            var process = System.Diagnostics.Process.GetProcessById((int)processId);
            return process.ProcessName;
        }
        catch
        {
            return "Unknown";
        }
    }

    private bool IsWindowMaximized(IntPtr hWnd)
    {
        // This is a simple heuristic - check if window covers most of the screen
        User32.GetWindowRect(hWnd, out var rect);
        var screen = Screen.FromHandle(hWnd);
        
        return rect.Width >= screen.WorkingArea.Width * 0.9 && 
               rect.Height >= screen.WorkingArea.Height * 0.9;
    }
}
