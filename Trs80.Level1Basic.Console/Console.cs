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
    private const int ScreenWidth = 64;
    private const int ScreenHeight = 16;
    private const int ScreenPixelWidth = 2 * ScreenWidth;
    private const int ScreenPixelHeight = 3 * ScreenHeight;
    private readonly bool[,] _screen = new bool[ScreenPixelWidth, ScreenPixelHeight];
    private readonly IAppSettings _appSettings;
    private readonly ISystemConsole _systemConsole;

    public Console(IAppSettings appSettings, ILoggerFactory logFactory, ISystemConsole systemConsole)
    {
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _systemConsole = systemConsole ?? throw new ArgumentNullException(nameof(systemConsole));
        if (logFactory == null) throw new ArgumentNullException(nameof(logFactory));
    }

    public int CursorX
    {
        get
        {
            (int left, _) = GetCursorPosition();
            return left;
        }
        set
        {
            SetCursorPosition(value, CursorY);
        }
    }

    public int CursorY
    {
        get
        {
            (_, int top) = GetCursorPosition();
            return top;
        }
        set
        {
            SetCursorPosition(CursorX, value);
        }
    }

    public TextReader In
    {
        get { return _systemConsole.In; }
        set { _systemConsole.In = value; }
    }

    public TextWriter Out
    {
        get { return _systemConsole.Out; }
        set { _systemConsole.Out = value; }
    }

    public TextWriter Error
    {
        get { return _systemConsole.Error; }
        set { _systemConsole.Error = value; }
    }

    public void WriteLine(string text = "") => _systemConsole.WriteLine(text);

    public void Write(string text) => _systemConsole.Write(text);

    public void Write(char c) => _systemConsole.Write(c);

    public string ReadLine() => _systemConsole.ReadLine();

    public void Clear()
    {
        _systemConsole.Clear();
        Erase(0, 0, ScreenPixelWidth - 1, ScreenPixelHeight - 1);
    }

    public void SetCursorPosition(int column, int row)
    {
        _systemConsole.SetCursorPosition(column, row);
    }

    public (int Left, int Top) GetCursorPosition()
    {
        return _systemConsole.GetCursorPosition();
    }

    public ConsoleFont GetCurrentFont()
    {
        return _systemConsole.GetCurrentConsoleFont();
    }

    public void SetCurrentFont(ConsoleFont font)
    {
        _systemConsole.SetCurrentConsoleFont(font);
    }

    public ConsoleKeyInfo ReadKey() => _systemConsole.ReadKey();
    public void InitializeWindow()
    {
        SetCurrentFont(new ConsoleFont { FontName = _appSettings.FontName, FontSize = _appSettings.FontSize });
        DisableCursorBlink();
        SetWindowSize(ScreenWidth, ScreenHeight);
        SetBufferSize(ScreenWidth, ScreenPixelHeight * 10);
    }

    private void SetWindowSize(int width, int height)
    {
        _systemConsole.SetWindowSize(width, height);
    }

    public void SetBufferSize(int width, int height)
    {
        _systemConsole.SetBufferSize(width, height);
    }

    private void DisableCursorBlink()
    {
        _systemConsole.EnableVirtualTerminal();

        Out.Write("\u001b[?12l");
    }

    private void Fill(int x, int y, int width, int height)
    {
        for (int xIndex = x; xIndex < x + width; xIndex++)
            for (int yIndex = y; yIndex < y + height; yIndex++)
                _screen[xIndex, yIndex] = true;

        _systemConsole.Fill(x, y, width, height);
    }

    private void Erase(int x, int y, int width, int height)
    {
        for (int xIndex = x; xIndex < x + width; xIndex++)
        for (int yIndex = y; yIndex < y + height; yIndex++)
            _screen[xIndex, yIndex] = false;

        _systemConsole.Erase(x, y, width, height);
    }

    public void Set(int x, int y)
    {
        Fill(x % ScreenPixelWidth, y % ScreenPixelHeight, 1, 1);
    }

    public void Reset(int x, int y)
    {
        Erase(x % ScreenPixelWidth, y % ScreenPixelHeight, 1, 1);
    }

    public bool Point(int x, int y)
    {
        return _screen[x % ScreenPixelWidth, y % ScreenPixelHeight];
    }
}