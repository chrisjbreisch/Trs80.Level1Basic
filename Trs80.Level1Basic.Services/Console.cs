using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Trs80Level1Basic.Win32Api;

namespace Trs80.Level1Basic.Services;

public class ConsoleFont
{
    public string FontName { get; set; }
    public short FontSize { get; set; }
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
public class Console : IConsole
{
    private readonly double _pixelWidth;
    private readonly double _pixelHeight;
    private readonly bool[,] _screen = new bool[128, 48];
    private readonly System.Drawing.Graphics _graphics;

    public TextWriter Out { get; set;  }
    public TextReader In { get; set; }
    public TextWriter Error { get; set; }


    public Console()
    {
        var hwnd = Win32Api.GetConsoleWindowHandle();
        var clientRect = Win32Api.GetClientRect(hwnd);
        _pixelHeight = clientRect.Bottom / 48.0;
        _pixelWidth = clientRect.Right / 128.0;
        _graphics = System.Drawing.Graphics.FromHwnd(hwnd);

        Out = System.Console.Out;
        In = System.Console.In;
        Error = System.Console.Error;
    }

    public void WriteLine(string text = "") => Out.WriteLine(text);

    public void Write(string text) => Out.Write(text);

    public string ReadLine() => In.ReadLine();

    public void Clear()
    {
        System.Console.Clear();
        //Fill(0, 0, 127, 47, false);
    }

    public void SetCursorPosition(int row, int column) => System.Console.SetCursorPosition(row, column);

    private const uint EnableVirtualTerminalProcessing = 4;
    private const int FixedWidthTrueType = 54;
    private const int StandardOutputHandle = -11;

    private static readonly IntPtr ConsoleOutputHandle = Win32Api.GetStdHandle(StandardOutputHandle);

    public ConsoleFont GetCurrentFont()
    {
        var font = new Win32Api.FontInfo
        {
            cbSize = Marshal.SizeOf<Win32Api.FontInfo>()
        };

        if (Win32Api.GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref font))
        {
            return new ConsoleFont { FontName = font.FontName, FontSize = font.FontSize };
        }
        else
        {
            int er = Marshal.GetLastWin32Error();
            throw new Win32Exception(er);
        }
    }

    public void SetCurrentFont(ConsoleFont font)
    {
        var before = new Win32Api.FontInfo
        {
            cbSize = Marshal.SizeOf<Win32Api.FontInfo>()
        };

        if (Win32Api.GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref before))
        {
            var set = new Win32Api.FontInfo
            {
                cbSize = Marshal.SizeOf<Win32Api.FontInfo>(),
                FontIndex = 0,
                FontFamily = FixedWidthTrueType,
                FontName = font.FontName,
                FontWeight = 400,
                FontSize = font.FontSize > 0 ? font.FontSize : before.FontSize
            };

            // Get some settings from current font.
            if (!Win32Api.SetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref set))
            {
                int ex = Marshal.GetLastWin32Error();
                throw new Win32Exception(ex);
            }

            var after = new Win32Api.FontInfo
            {
                cbSize = Marshal.SizeOf<Win32Api.FontInfo>()
            };
            Win32Api.GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref after);
        }
        else
        {
            int er = Marshal.GetLastWin32Error();
            throw new Win32Exception(er);
        }
    }

    public ConsoleKeyInfo ReadKey() => System.Console.ReadKey();

    public void SetWindowSize(int width, int height)
    {
        if (OperatingSystem.IsWindows())
            System.Console.SetWindowSize(width, height);
    }

    public void SetBufferSize(int width, int height)
    {
        if (OperatingSystem.IsWindows())
            System.Console.SetBufferSize(width, height);
    }
        
    public void DisableCursorBlink()
    {
        if (!Win32Api.GetConsoleMode(ConsoleOutputHandle, out uint lpMode))
        {
            int ex = Marshal.GetLastWin32Error();
            throw new Win32Exception(ex);
        }

        lpMode |= EnableVirtualTerminalProcessing;
        if (!Win32Api.SetConsoleMode(ConsoleOutputHandle, lpMode))
        {
            int ex = Marshal.GetLastWin32Error();
            throw new Win32Exception(ex);
        }
        Out.Write("\u001b[?12l");
    }

    private void Fill(int x, int y, int width, int height, bool turnOn)
    {
        for (int xIndex = x; xIndex < x + width; xIndex++)
        for (int yIndex = y; yIndex < y + height; yIndex++)
            _screen[xIndex, yIndex] = turnOn;
        //if (_hwnd == IntPtr.Zero) Initialize();

        //using var graphics = System.Drawing.Graphics.FromHwnd(_hwnd);
        _graphics.FillRectangle(
            turnOn ? Brushes.White : Brushes.Black,
            (int)Math.Round(x * _pixelWidth),
            (int)Math.Round(y * _pixelHeight),
            (int)Math.Round(width * _pixelWidth),
            (int)Math.Round(height * _pixelHeight)
        );
    }

    public void Set(int x, int y)
    {
        Fill(x, y, 1, 1, true);
    }

    public void Reset(int x, int y)
    {
        Fill(x, y, 1, 1, false);
    }

    public bool Point(int x, int y)
    {
        return _screen[x, y];
    }
}