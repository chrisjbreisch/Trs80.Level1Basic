using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Trs80.Level1Basic.Win32Api;

public static class Win32Api
{
    // https://www.pinvoke.net/default.aspx/user32/FindWindow.html
    [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr FindWindowFromWindowName(IntPtr zeroOnly, string lpWindowName);

    // https://www.pinvoke.net/default.aspx/user32/GetClientRect.html
    [DllImport("user32.dll")]
    public static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);

    // https://www.pinvoke.net/default.aspx/kernel32/GetConsoleMode.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    // https://www.pinvoke.net/default.aspx/kernel32/GetConsoleTitle.html
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern uint GetConsoleTitle([Out] StringBuilder lpConsoleTitle, uint nSize);

    // https://www.pinvoke.net/default.aspx/kernel32/GetCurrentConsoleFontEx.html
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool GetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool maximumWindow, ref FontInfo consoleCurrentFontEx);

    // https://www.pinvoke.net/default.aspx/kernel32/GetStdHandle.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetStdHandle(int nStdHandle);

    // https://www.pinvoke.net/default.aspx/kernel32/SetConsoleMode.html
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    // https://www.pinvoke.net/default.aspx/kernel32/SetConsoleTitle.html
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool SetConsoleTitle(string lpConsoleTitle);

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool maximumWindow, ref FontInfo consoleCurrentFontEx);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct FontInfo
    {
        public int cbSize;
        public int FontIndex;
        public short FontWidth;
        public short FontSize;
        public int FontFamily;
        public int FontWeight;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string FontName;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left;        // x position of upper-left corner
        public int Top;         // y position of upper-left corner
        public int Right;       // x position of lower-right corner
        public int Bottom;      // y position of lower-right corner
    }

    public static IntPtr GetConsoleWindowHandle()
    {
        var sb = new StringBuilder(1024);
        _ = GetConsoleTitle(sb, 1024);

        string originalTitle = sb.ToString();

        string newTitle = originalTitle + Process.GetCurrentProcess().Id;
        SetConsoleTitle(newTitle);

        IntPtr hwnd;
        do
            hwnd = FindWindowFromWindowName(IntPtr.Zero, newTitle);
        while (!IsValidHwnd(hwnd));

        SetConsoleTitle(originalTitle);
        return hwnd;
    }

    private static bool IsValidHwnd(IntPtr hwnd)
    {
        if (hwnd != IntPtr.Zero) return true;

        Thread.Sleep(100);
        return false;
    }

    public static Rect GetClientRect(IntPtr hWnd)
    {
        GetClientRect(hWnd, out Rect result);
        return result;
    }
}