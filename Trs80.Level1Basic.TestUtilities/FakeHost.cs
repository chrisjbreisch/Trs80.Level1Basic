using Trs80.Level1Basic.HostMachine;
using System.Collections.Generic;

namespace Trs80.Level1Basic.TestUtilities;

public class FakeHost : IHost
{
    private const int ScreenWidth = 64;
    private const int ScreenHeight = 16;
    private const int ScreenPixelWidth = 2 * ScreenWidth;
    private const int ScreenPixelHeight = 3 * ScreenHeight;
    private readonly bool[,] _screen = new bool[ScreenPixelWidth, ScreenPixelHeight];
    private int _cursorX;
    private int _cursorY;
    private readonly Queue<ConsoleKeyInfo> _queuedKeys = new();

    public int BeepCount { get; private set; }
    public int ReadKeyCount { get; private set; }
    public List<string> PrintedDocuments { get; } = new();
    public string FileNameForLoad { get; set; } = string.Empty;
    public string FileNameForSave { get; set; } = string.Empty;

    public FakeHost()
    {
        Clear();
    }

    public void EnableVirtualTerminal()
    {
        // do nothing
    }

    public HostFont GetCurrentConsoleFont()
    {
        return new HostFont { FontSize = 12, FontName = "Courier New" };
    }

    public void SetCurrentConsoleFont(HostFont font)
    {
        // do nothing
    }
    public (int Left, int Top) GetCursorPosition()
    {
        return (_cursorX, _cursorY);
    }


    public void SetCursorPosition(int column, int row)
    {
        _cursorX = column;
        _cursorY = row;
    }

    public void Clear()
    {
        Erase(0, 0, ScreenPixelWidth, ScreenPixelHeight);
        _cursorX = 0;
        _cursorY = 0;
    }

    public void Beep()
    {
        BeepCount++;
    }

    public ConsoleKeyInfo ReadKey()
    {
        ReadKeyCount++;
        return _queuedKeys.Count > 0 ? _queuedKeys.Dequeue() : new ConsoleKeyInfo();
    }

    public bool TryReadKey(out ConsoleKeyInfo key)
    {
        if (_queuedKeys.Count == 0)
        {
            key = default;
            return false;
        }

        key = _queuedKeys.Dequeue();
        return true;
    }

    public void EnqueueKey(ConsoleKeyInfo key)
    {
        _queuedKeys.Enqueue(key);
    }

    public void SetWindowSize(int width, int height)
    {
        // do nothing
    }

    public void SetBufferSize(int width, int height)
    {
        // do nothing
    }

    public void Fill(int x, int y, int width, int height)
    {
        for (int xIndex = x; xIndex < x + width; xIndex++)
        for (int yIndex = y; yIndex < y + height; yIndex++)
            _screen[xIndex, yIndex] = true;
    }

    public void Erase(int x, int y, int width, int height)
    {
        for (int xIndex = x; xIndex < x + width; xIndex++)
        for (int yIndex = y; yIndex < y + height; yIndex++)
            _screen[xIndex, yIndex] = false;
    }

    public TextWriter Out { get; set; } = Console.Out;
    public TextReader In { get; set; } = Console.In;
    public TextWriter Error { get; set; } = Console.Error;
    public void WriteLine(string text = "")
    {
        int startingRow = _cursorY;
        Write(text);
        Out.WriteLine();
        if (_cursorY == startingRow || _cursorX != 0)
            _cursorY++;
        _cursorX = 0;
    }

    public void Write(string text)
    {
        Out.Write(text);
        int totalColumns = _cursorX + text.Length;
        _cursorY += totalColumns / ScreenWidth;
        _cursorX = totalColumns % ScreenWidth;
    }

    public void Print(string text)
    {
        PrintedDocuments.Add(text);
    }

    public string ReadLine()
    {
        return In.ReadLine()!.ToUpperInvariant();
    }

    public string GetFileNameForSave()
    {
        return FileNameForSave;
    }

    public string GetFileNameForLoad()
    {
        return FileNameForLoad;
    }
}