using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Trs80.Level1Basic.Common;
using Trs80.Level1Basic.HostMachine;
using Trs80.Level1Basic.VirtualMachine.Exceptions;
using Trs80.Level1Basic.VirtualMachine.Interpreter;
using Trs80.Level1Basic.VirtualMachine.Parser.Statements;

namespace Trs80.Level1Basic.VirtualMachine.Environment;

public interface ITrs80
{
    Statement CurrentStatement { get; set; }

    int Int(dynamic value);
    dynamic Mem();
    dynamic Abs(dynamic value);
    dynamic Chr(dynamic value);
    dynamic Rnd(int control);
    string Tab(dynamic value);
    string PadQuadrant();
    object Set(float x, float y);
    object Reset(float x, float y);
    int Point(int x, int y);

    int CursorX { get; set; }
    int CursorY { get; set; }
    TextWriter Out { get; set; }
    TextReader In { get; set; }
    TextWriter Error { get; set; }
    void WriteLine(string text = "");
    void Write(string text);
    string ReadLine();
    void Clear();
    void SetCursorPosition(int column, int row);
    (int Left, int Top) GetCursorPosition();
    ConsoleFont GetCurrentFont();
    void SetCurrentFont(ConsoleFont font);
    ConsoleKeyInfo ReadKey();
    void InitializeWindow();
    string PadToPosition(int value);
}

public class Trs80 : ITrs80
{
    private readonly IProgram _program;
    private const int ScreenWidth = 64;
    private const int ScreenHeight = 16;
    private const int ScreenPixelWidth = 2 * ScreenWidth;
    private const int ScreenPixelHeight = 3 * ScreenHeight;
    private readonly bool[,] _screen = new bool[ScreenPixelWidth, ScreenPixelHeight];
    private readonly IAppSettings _appSettings;
    private readonly IHost _host;
    public Statement CurrentStatement { get; set; }


    public Trs80(IProgram program, IAppSettings appSettings, ILoggerFactory logFactory, IHost host)
    {
        _program = program ?? throw new ArgumentNullException(nameof(program));
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _host = host ?? throw new ArgumentNullException(nameof(host));
        if (logFactory == null) throw new ArgumentNullException(nameof(logFactory));
    }

    public int Int(dynamic value)
    {
        if (Math.Abs(value) > short.MaxValue)
            throw new ValueOutOfRangeException(_program.CurrentStatement.LineNumber, _program.CurrentStatement.SourceLine,
                $"{value} is out of range for int().");
        return (int)Math.Floor((float)value);
    }

    private const int TotalMemory = 3583 + 12 * 1024;
    public dynamic Mem()
    {
        return TotalMemory - _program.Size();
    }

    public dynamic Abs(dynamic value)
    {
        if (value is float fValue)
            return Math.Abs(fValue);
        return Math.Abs((int)value);
    }

    public dynamic Chr(dynamic value)
    {
        return (char)value;
    }

    private static readonly Random Rand = new();
    public dynamic Rnd(int control)
    {
        if (control == 0)
            return (float)Rand.NextDouble();

        return (int)Math.Floor(control * Rand.NextDouble() + 1);
    }

    public string Tab(dynamic value)
    {
        return PadToPosition(value);
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
            if (value > ScreenWidth)
            {
                value %= ScreenWidth;
                CursorY++;
            }
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
        get { return _host.In; }
        set { _host.In = value; }
    }

    public TextWriter Out
    {
        get { return _host.Out; }
        set { _host.Out = value; }
    }

    public TextWriter Error
    {
        get { return _host.Error; }
        set { _host.Error = value; }
    }

    public void WriteLine(string text = "") => _host.WriteLine(text);

    public void Write(string text) => _host.Write(text);

    public void Write(char c) => _host.Write(c);

    public string ReadLine() => _host.ReadLine();

    public void Clear()
    {
        _host.Clear();
        Erase(0, 0, ScreenPixelWidth - 1, ScreenPixelHeight - 1);
    }

    public void SetCursorPosition(int column, int row)
    {
        _host.SetCursorPosition(column, row);
    }

    public (int Left, int Top) GetCursorPosition()
    {
        return _host.GetCursorPosition();
    }

    public ConsoleFont GetCurrentFont()
    {
        return _host.GetCurrentConsoleFont();
    }

    public void SetCurrentFont(ConsoleFont font)
    {
        _host.SetCurrentConsoleFont(font);
    }

    public ConsoleKeyInfo ReadKey() => _host.ReadKey();
    public void InitializeWindow()
    {
        SetCurrentFont(new ConsoleFont { FontName = _appSettings.FontName, FontSize = _appSettings.FontSize });
        DisableCursorBlink();
        SetWindowSize(ScreenWidth, ScreenHeight);
        SetBufferSize(ScreenWidth, ScreenPixelHeight * 10);
    }

    private void SetWindowSize(int width, int height)
    {
        _host.SetWindowSize(width, height);
    }

    public void SetBufferSize(int width, int height)
    {
        _host.SetBufferSize(width, height);
    }

    private void DisableCursorBlink()
    {
        _host.EnableVirtualTerminal();

        Out.Write("\u001b[?12l");
    }

    private void Fill(int x, int y, int width, int height)
    {
        for (int xIndex = x; xIndex < x + width; xIndex++)
            for (int yIndex = y; yIndex < y + height; yIndex++)
                _screen[xIndex, yIndex] = true;

        _host.Fill(x, y, width, height);
    }

    private void Erase(int x, int y, int width, int height)
    {
        for (int xIndex = x; xIndex < x + width; xIndex++)
            for (int yIndex = y; yIndex < y + height; yIndex++)
                _screen[xIndex, yIndex] = false;

        _host.Erase(x, y, width, height);
    }

    public object Set(float x, float y)
    {
        Fill((int)x % ScreenPixelWidth, (int)y % ScreenPixelHeight, 1, 1);
        return null!;
    }

    public object Reset(float x, float y)
    {
        Erase((int)x % ScreenPixelWidth, (int)y % ScreenPixelHeight, 1, 1);
        return null!;
    }

    public int Point(int x, int y)
    {
        return _screen[x % ScreenPixelWidth, y % ScreenPixelHeight] ? 1 : 0;
    }

    public string PadToPosition(int position)
    {
        if (CursorX > position) return "";

        string padding = "".PadRight(position - CursorX, ' ');

        return padding;
    }

    public string PadQuadrant()
    {
        int nextPosition = (CursorX / 16 + 1) * 16;
        string padding = "".PadRight(nextPosition - CursorX, ' ');
        return padding;
    }
}