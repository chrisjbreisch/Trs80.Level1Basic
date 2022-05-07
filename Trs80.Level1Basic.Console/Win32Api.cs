using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Trs80.Level1Basic.Console;

public static class Win32Api
{
    // https://www.pinvoke.net/default.aspx/user32/FindWindow.html
    [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr FindWindowFromWindowName(IntPtr zeroOnly, string lpWindowName);

    // https://www.pinvoke.net/default.aspx/user32/GetClientRect.html
    [DllImport("user32.dll")]
    private static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);

    // https://www.pinvoke.net/default.aspx/kernel32/GetConsoleMode.html
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    // https://www.pinvoke.net/default.aspx/kernel32/GetConsoleTitle.html
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern uint GetConsoleTitle([Out] StringBuilder lpConsoleTitle, uint nSize);

    // https://www.pinvoke.net/default.aspx/kernel32/GetCurrentConsoleFontEx.html
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool GetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool maximumWindow, ref FontInfo consoleCurrentFontEx);

    // https://www.pinvoke.net/default.aspx/kernel32/GetStdHandle.html
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    // https://www.pinvoke.net/default.aspx/kernel32/SetConsoleMode.html
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    // https://www.pinvoke.net/default.aspx/kernel32/SetConsoleTitle.html
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool SetConsoleTitle(string lpConsoleTitle);

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool maximumWindow, ref FontInfo consoleCurrentFontEx);

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
    private static IntPtr GetConsoleWindowHandle()
    {
        var sb = new StringBuilder(1024);
        _ = GetConsoleTitle(sb, 1024);

        string originalTitle = sb.ToString();

        string newTitle = originalTitle + Environment.ProcessId;
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

    public static Rect GetClientRect()
    {
        GetClientRect(_hwnd, out Rect result);
        return result;
    }

    private const uint EnableVirtualTerminalProcessing = 4;
    private const int FixedWidthTrueType = 54;
    private const int StandardOutputHandle = -11;

    public static void EnableVirtualTerminal()
    {
        int ex;
        if (!GetConsoleMode(_outputHandle, out uint lpMode))
        {
            ex = Marshal.GetLastWin32Error();
            throw new Win32Exception(ex);
        }

        lpMode |= EnableVirtualTerminalProcessing;
        if (SetConsoleMode(_outputHandle, lpMode)) return;

        ex = Marshal.GetLastWin32Error();
        throw new Win32Exception(ex);
    }

    private static readonly IntPtr _hwnd = GetConsoleWindowHandle();
    private static readonly IntPtr _outputHandle = GetStdHandle(StandardOutputHandle);

    public static ConsoleFont GetCurrentConsoleFont()
    {
        var font = new FontInfo
        {
            cbSize = Marshal.SizeOf<FontInfo>()
        };

        if (GetCurrentConsoleFontEx(_outputHandle, false, ref font))
            return new ConsoleFont { FontName = font.FontName, FontSize = font.FontSize };

        int er = Marshal.GetLastWin32Error();
        throw new Win32Exception(er);
    }

    public static void SetCurrentConsoleFont(ConsoleFont font)
    {
        var before = new FontInfo
        {
            cbSize = Marshal.SizeOf<FontInfo>()
        };

        if (GetCurrentConsoleFontEx(_outputHandle, false, ref before))
        {
            var set = new FontInfo
            {
                cbSize = Marshal.SizeOf<FontInfo>(),
                FontIndex = 0,
                FontFamily = FixedWidthTrueType,
                FontName = font.FontName,
                FontWeight = 400,
                FontSize = font.FontSize > 0 ? font.FontSize : before.FontSize
            };

            if (!SetCurrentConsoleFontEx(_outputHandle, false, ref set))
            {
                int ex = Marshal.GetLastWin32Error();
                throw new Win32Exception(ex);
            }
        }
        else
        {
            int er = Marshal.GetLastWin32Error();
            throw new Win32Exception(er);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Only used on Windows")]
    public static Graphics GetGraphics()
    {
        return Graphics.FromHwnd(_hwnd);
    }
}