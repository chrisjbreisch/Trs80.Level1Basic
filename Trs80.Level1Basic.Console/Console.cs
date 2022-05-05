using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Trs80.Level1Basic.Common;

namespace Trs80.Level1Basic.Console;

public class ConsoleFont
{
    public string FontName { get; set; }
    public short FontSize { get; set; }
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility",
    Justification = "It's fine if these pieces don't work on non-Windows devices")]
public class Console : IConsole
{
    private IntPtr _hwnd;
    private double _pixelWidth;
    private double _pixelHeight;
    private const int ScreenWidth = 64;
    private const int ScreenHeight = 16;
    private const int ScreenPixelWidth = 2 * ScreenWidth;
    private const int ScreenPixelHeight = 3 * ScreenHeight;
    private readonly bool[,] _screen = new bool[ScreenPixelWidth, ScreenPixelHeight];
    private readonly Graphics _graphics;
    private readonly IAppSettings _appSettings;

    public TextWriter Out { get; set; }
    public TextReader In { get; set; }
    public TextWriter Error { get; set; }

    public Console(IAppSettings appSettings)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

        _hwnd = Win32Api.Win32Api.GetConsoleWindowHandle();
        SetPixelSizes();
        _graphics = Graphics.FromHwnd(_hwnd);

        Out = System.Console.Out;
        In = System.Console.In;
        Error = System.Console.Error;
    }

    private void SetPixelSizes()
    {
        Win32Api.Win32Api.Rect clientRect = Win32Api.Win32Api.GetClientRect(_hwnd);
        _pixelHeight = clientRect.Bottom / (double) ScreenPixelHeight;
        _pixelWidth = clientRect.Right / (double) ScreenPixelWidth;
    }

    public void WriteLine(string text = "") => Out.WriteLine(text);

    public void Write(string text) => Out.Write(text);

    public void Write(char c) => Out.Write(c);

    public string ReadLine() => In.ReadLine();

    public void Clear()
    {
        System.Console.Clear();
        Fill(0, 0, ScreenPixelWidth - 1, ScreenPixelHeight - 1, false);
    }

    public void SetCursorPosition(int column, int row)
    {
        System.Console.SetCursorPosition(column, row);
    }

    public (int Left, int Top) GetCursorPosition()
    {
        return System.Console.GetCursorPosition();
    }

    private const uint EnableVirtualTerminalProcessing = 4;
    private const int FixedWidthTrueType = 54;
    private const int StandardOutputHandle = -11;

    private static readonly IntPtr ConsoleOutputHandle = Win32Api.Win32Api.GetStdHandle(StandardOutputHandle);

    public ConsoleFont GetCurrentFont()
    {
        var font = new Win32Api.Win32Api.FontInfo
        {
            cbSize = Marshal.SizeOf<Win32Api.Win32Api.FontInfo>()
        };

        if (Win32Api.Win32Api.GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref font))
            return new ConsoleFont { FontName = font.FontName, FontSize = font.FontSize };

        int er = Marshal.GetLastWin32Error();
        throw new Win32Exception(er);
    }

    public void SetCurrentFont(ConsoleFont font)
    {
        var before = new Win32Api.Win32Api.FontInfo
        {
            cbSize = Marshal.SizeOf<Win32Api.Win32Api.FontInfo>()
        };

        if (Win32Api.Win32Api.GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref before))
        {
            var set = new Win32Api.Win32Api.FontInfo
            {
                cbSize = Marshal.SizeOf<Win32Api.Win32Api.FontInfo>(),
                FontIndex = 0,
                FontFamily = FixedWidthTrueType,
                FontName = font.FontName,
                FontWeight = 400,
                FontSize = font.FontSize > 0 ? font.FontSize : before.FontSize
            };

            if (!Win32Api.Win32Api.SetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref set))
            {
                int ex = Marshal.GetLastWin32Error();
                throw new Win32Exception(ex);
            }

            var after = new Win32Api.Win32Api.FontInfo
            {
                cbSize = Marshal.SizeOf<Win32Api.Win32Api.FontInfo>()
            };
            Win32Api.Win32Api.GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref after);
        }
        else
        {
            int er = Marshal.GetLastWin32Error();
            throw new Win32Exception(er);
        }
    }

    public ConsoleKeyInfo ReadKey() => System.Console.ReadKey();
    public void InitializeWindow()
    {
        SetCurrentFont(new ConsoleFont { FontName = _appSettings.FontName, FontSize = _appSettings.FontSize });
        DisableCursorBlink();
        SetWindowSize(ScreenWidth, ScreenHeight);
        SetBufferSize(ScreenWidth, ScreenPixelHeight * 10);
    }

    private void SetWindowSize(int width, int height)
    {
        if (OperatingSystem.IsWindows())
        {
            System.Console.SetWindowSize(width, height);
            SetPixelSizes();
        }
    }

    private void SetBufferSize(int width, int height)
    {
        if (OperatingSystem.IsWindows())
            System.Console.SetBufferSize(width, height);
    }

    private void DisableCursorBlink()
    {
        if (!Win32Api.Win32Api.GetConsoleMode(ConsoleOutputHandle, out uint lpMode))
        {
            int ex = Marshal.GetLastWin32Error();
            throw new Win32Exception(ex);
        }

        lpMode |= EnableVirtualTerminalProcessing;
        if (!Win32Api.Win32Api.SetConsoleMode(ConsoleOutputHandle, lpMode))
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

        _graphics.FillRectangle(
            turnOn ? Brushes.White : Brushes.Black,
            (int)(x * _pixelWidth),
            (int)(y * _pixelHeight),
            (int)Math.Round(width * _pixelWidth + .5),
            (int)Math.Round(height * _pixelHeight + .5)
        );
    }

    public void Set(int x, int y)
    {
        Fill(x % ScreenPixelWidth, y % ScreenPixelHeight, 1, 1, true);
    }

    public void Reset(int x, int y)
    {
        Fill(x % ScreenPixelWidth, y % ScreenPixelHeight, 1, 1, false);
    }

    public bool Point(int x, int y)
    {
        return _screen[x % ScreenPixelWidth, y % ScreenPixelHeight];
    }
}