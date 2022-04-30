using System;
using System.Drawing;
using Trs80Level1Basic.Win32Api;

namespace Trs80.Level1Basic.Graphics;

public interface IScreen
{
    void Set(int x, int y);
    void Reset(int x, int y);
    bool Point(int x, int y);
    void Clear();
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
public class Screen : IScreen
{
    private readonly double _pixelWidth;
    private readonly double _pixelHeight;
    private readonly bool[,] _screen = new bool[128,48];
    private readonly System.Drawing.Graphics _graphics;
    
    public Screen()
    {
        var hwnd = Win32Api.GetConsoleWindowHandle();
        var clientRect = Win32Api.GetClientRect(hwnd);
        _pixelHeight = clientRect.Bottom / 48.0;
        _pixelWidth = clientRect.Right / 128.0;
        _graphics = System.Drawing.Graphics.FromHwnd(hwnd);
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

    public void Clear()
    {
        Fill(0, 0, 127, 47, false);
    }

    public bool Point(int x, int y)
    {
        return _screen[x, y];
    }
}