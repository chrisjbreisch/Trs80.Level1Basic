using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Trs80.Level1Basic.Console;

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

public interface ISystemConsole
{
    void EnableVirtualTerminal();
    ConsoleFont GetCurrentConsoleFont();
    void SetCurrentConsoleFont(ConsoleFont font);
    (int Left, int Top) GetCursorPosition();
    void SetCursorPosition(int column, int row);
    void Clear();
    ConsoleKeyInfo ReadKey();
    void SetWindowSize(int width, int height);
    void SetBufferSize(int width, int height);
    void Fill(int x, int y, int width, int height);
    void Erase(int x, int y, int width, int height);
    TextWriter Out { get; set; }
    TextReader In { get; set; }
    TextWriter Error { get; set; }
    void WriteLine(string text = "");
    void Write(string text);
    void Write(char c);
    string ReadLine();
}

public class SystemConsole : ISystemConsole, IDisposable 
{
    // https://www.pinvoke.net/default.aspx/user32/FindWindow.html
    [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern nint FindWindowFromWindowName(nint zeroOnly, string lpWindowName);

    // https://www.pinvoke.net/default.aspx/user32/GetClientRect.html
    [DllImport("user32.dll")]
    private static extern bool GetClientRect(nint hWnd, out Rect lpRect);

    // https://www.pinvoke.net/default.aspx/kernel32/GetConsoleMode.html
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetConsoleMode(nint hConsoleHandle, out uint lpMode);

    // https://www.pinvoke.net/default.aspx/kernel32/GetConsoleTitle.html
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern uint GetConsoleTitle([Out] StringBuilder lpConsoleTitle, uint nSize);

    // https://www.pinvoke.net/default.aspx/kernel32/GetCurrentConsoleFontEx.html
    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool GetCurrentConsoleFontEx(nint hConsoleOutput, bool maximumWindow, ref FontInfo consoleCurrentFontEx);

    // https://www.pinvoke.net/default.aspx/kernel32/GetStdHandle.html
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint GetStdHandle(int nStdHandle);

    // https://www.pinvoke.net/default.aspx/kernel32/SetConsoleMode.html
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleMode(nint hConsoleHandle, uint dwMode);

    // https://www.pinvoke.net/default.aspx/kernel32/SetConsoleTitle.html
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool SetConsoleTitle(string lpConsoleTitle);

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool SetCurrentConsoleFontEx(nint hConsoleOutput, bool maximumWindow, ref FontInfo consoleCurrentFontEx);

    private const int ScreenWidth = 64;
    private const int ScreenHeight = 16;
    private const int ScreenPixelWidth = 2 * ScreenWidth;
    private const int ScreenPixelHeight = 3 * ScreenHeight;

    private readonly nint _hwnd;
    private readonly nint _outputHandle;
    private readonly Graphics _graphics;
    private double _pixelWidth;
    private double _pixelHeight;

    public TextWriter Out { get; set; } = System.Console.Out;
    public TextReader In { get; set; } = System.Console.In;
    public TextWriter Error { get; set; } = System.Console.Error;

    public SystemConsole()
    {
        _hwnd = GetConsoleWindowHandle();
        _outputHandle = GetStdHandle(StandardOutputHandle);
        _graphics = GetGraphics();
        SetPixelSizes();
    }

    public void WriteLine(string text = "") => Out.WriteLine(text);

    public void Write(string text) => Out.Write(text);

    public void Write(char c) => Out.Write(c);

    public string ReadLine() => In.ReadLine();


    private void SetPixelSizes()
    {
        Rect clientRect = GetClientRect();
        _pixelHeight = clientRect.Bottom / (double)ScreenPixelHeight;
        _pixelWidth = clientRect.Right / (double)ScreenPixelWidth;
    }

    private static nint GetConsoleWindowHandle()
    {
        var sb = new StringBuilder(1024);
        _ = GetConsoleTitle(sb, 1024);

        string originalTitle = sb.ToString();

        string newTitle = originalTitle + Environment.ProcessId;
        SetConsoleTitle(newTitle);

        nint hwnd;
        do
            hwnd = FindWindowFromWindowName(IntPtr.Zero, newTitle);
        while (!IsValidHwnd(hwnd));

        SetConsoleTitle(originalTitle);

        return hwnd;
    }

    private static bool IsValidHwnd(nint hwnd)
    {
        if (hwnd != IntPtr.Zero) return true;

        Thread.Sleep(100);
        return false;
    }

    public Rect GetClientRect()
    {
        GetClientRect(_hwnd, out Rect result);
        return result;
    }

    private const uint EnableVirtualTerminalProcessing = 4;
    private const int FixedWidthTrueType = 54;
    private const int StandardOutputHandle = -11;

    public void EnableVirtualTerminal()
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


    public ConsoleFont GetCurrentConsoleFont()
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

    public void SetCurrentConsoleFont(ConsoleFont font)
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
    public Graphics GetGraphics()
    {
        return Graphics.FromHwnd(_hwnd);
    }

    public (int Left, int Top) GetCursorPosition()
    {
        return System.Console.GetCursorPosition();
    }

    public void SetCursorPosition(int column, int row)
    {
        System.Console.SetCursorPosition(column, row);
    }

    public void Clear()
    {
        System.Console.Clear();
    }

    public ConsoleKeyInfo ReadKey()
    {
        return System.Console.ReadKey();
    }

    public void SetWindowSize(int width, int height)
    {
        if (!OperatingSystem.IsWindows()) return;

        System.Console.SetWindowSize(width, height);
        SetPixelSizes();
    }

    public void SetBufferSize(int width, int height)
    {
        if (OperatingSystem.IsWindows())
            System.Console.SetBufferSize(width, height);
    }

    public void Fill(int x, int y, int width, int height)
    {
        if (!OperatingSystem.IsWindows()) return;

        _graphics.FillRectangle(
            Brushes.White,
            (int)(x * _pixelWidth),
            (int)(y * _pixelHeight),
            (int)Math.Round(width * _pixelWidth + .5),
            (int)Math.Round(height * _pixelHeight + .5)
        );

    }

    public void Erase(int x, int y, int width, int height)
    {
        if (!OperatingSystem.IsWindows()) return;

        _graphics.FillRectangle(
            Brushes.Black,
            (int)(x * _pixelWidth),
            (int)(y * _pixelHeight),
            (int)Math.Round(width * _pixelWidth + .5),
            (int)Math.Round(height * _pixelHeight + .5)
        );
    }


    private void Dispose(bool disposing)
    {
        if (!disposing) return;

        if (OperatingSystem.IsWindows())
            _graphics?.Dispose();

        Out?.Dispose();
        In?.Dispose();
        Error?.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~SystemConsole()
    {
        Dispose(false);
    }
}