using System;
using System.IO;

using Trs80.Level1Basic.HostMachine;

namespace Trs80.Level1Basic.VirtualMachine.Machine;

public interface ITrs80
{
    string PadQuadrant();
    object Set(float x, float y);
    object Reset(float x, float y);
    int Point(int x, int y);

    int CursorX { get; set; }
    int CursorY { get; set; }
    TextWriter Out { get; set; }
    TextWriter Error { get; set; }
    TextReader In { get; set; }
    void WriteLine(string text = "");
    void Write(string text);
    string PadToPosition(int position);
    string ReadLine();
    void Clear();
    void SetCursorPosition(int column, int row);
    (int Left, int Top) GetCursorPosition();
    HostFont GetCurrentFont();
    void SetCurrentFont(HostFont font);
    ConsoleKeyInfo ReadKey();
    void InitializeWindow();
}