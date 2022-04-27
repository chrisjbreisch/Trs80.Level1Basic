using System;
using System.Drawing;

namespace Trs80.Level1Basic.Graphics;

public interface IScreen
{
    void Set(int x, int y);
    void Reset(int x, int y);
    bool Point(int x, int y);
    void Clear();
}

public class Screen : IScreen
{
    private IntPtr _hwnd;
    private double _pixelWidth;
    private double _pixelHeight;
    private readonly bool[,] _screen = new bool[128,48];

    private void Initialize()
    {
        _hwnd = Win32Api.GetConsoleWindowHandle();
        var clientRect = Win32Api.GetClientRect(_hwnd);
        _pixelHeight = clientRect.Bottom / 48.0;
        _pixelWidth = clientRect.Right / 128.0;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public void Set(int x, int y)
    {
        if (_hwnd == IntPtr.Zero) Initialize();

        _screen[x, y] = true;
        using var graphics = System.Drawing.Graphics.FromHwnd(_hwnd);
        graphics.FillRectangle(
            Brushes.White, 
            (int)Math.Round(x * _pixelWidth), 
            (int)Math.Round(y * _pixelHeight), 
            (int)Math.Round(_pixelWidth), 
            (int)Math.Round(_pixelHeight));

    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public void Reset(int x, int y)
    {
        if (_hwnd == IntPtr.Zero) Initialize();

        _screen[x, y] = false;
        using var graphics = System.Drawing.Graphics.FromHwnd(_hwnd);
        graphics.FillRectangle(
            Brushes.Black,
            (int)Math.Round(x * _pixelWidth),
            (int)Math.Round(y * _pixelHeight),
            (int)Math.Round(_pixelWidth),
            (int)Math.Round(_pixelHeight));
    }

    public void Clear()
    {
        for (int x = 0; x < 128; x++)
        for (int y = 0; y < 48; y++)
            if (_screen[x, y])
                Reset(x, y);
    }

    public bool Point(int x, int y)
    {
        return _screen[x, y];
    }
}