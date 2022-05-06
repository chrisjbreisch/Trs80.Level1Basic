using System;
using System.Drawing;
using System.IO;

using Microsoft.Extensions.Logging;

using Trs80.Level1Basic.Common;

namespace Trs80.Level1Basic.Console;



[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility",
    Justification = "It's fine if these pieces don't work on non-Windows devices")]
public class Console : IConsole
{
    private readonly IntPtr _hwnd;
    private double _pixelWidth;
    private double _pixelHeight;
    private const int ScreenWidth = 64;
    private const int ScreenHeight = 16;
    private const int ScreenPixelWidth = 2 * ScreenWidth;
    private const int ScreenPixelHeight = 3 * ScreenHeight;
    private readonly bool[,] _screen = new bool[ScreenPixelWidth, ScreenPixelHeight];
    private Graphics _graphics;
    private readonly IAppSettings _appSettings;
    private readonly ILogger _logger;

    public TextWriter Out { get; set; }
    public TextReader In { get; set; }
    public TextWriter Error { get; set; }

    public Console(IAppSettings appSettings, ILoggerFactory logFactory)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        if (logFactory == null) throw new ArgumentNullException(nameof(logFactory));
        _logger = logFactory.CreateLogger<Console>();

        _hwnd = Win32Api.GetConsoleWindowHandle();
        InitializeWindowSettings();

        Out = System.Console.Out;
        In = System.Console.In;
        Error = System.Console.Error;
    }

    private void InitializeWindowSettings()
    {
        Win32Api.Rect clientRect = Win32Api.GetClientRect(_hwnd);
        _pixelHeight = clientRect.Bottom / (double)ScreenPixelHeight;
        _pixelWidth = clientRect.Right / (double)ScreenPixelWidth;
        _logger.LogDebug($"_pixelHeight: {_pixelHeight}, _pixelWidth: {_pixelWidth}");
        _graphics = Graphics.FromHwnd(_hwnd);
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

    private static readonly IntPtr ConsoleOutputHandle = Win32Api.GetConsoleWindowHandle();

    public ConsoleFont GetCurrentFont()
    {
        return Win32Api.GetCurrentConsoleFont(ConsoleOutputHandle);
    }

    public void SetCurrentFont(ConsoleFont font)
    {
        Win32Api.SetCurrentConsoleFont(ConsoleOutputHandle, font);
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
        if (!OperatingSystem.IsWindows()) return;

        System.Console.SetWindowSize(width, height);
        InitializeWindowSettings();
    }

    public void SetBufferSize(int width, int height)
    {
        if (OperatingSystem.IsWindows())
            System.Console.SetBufferSize(width, height);
    }

    private void DisableCursorBlink()
    {
        Win32Api.EnableVirtualTerminal(ConsoleOutputHandle);

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