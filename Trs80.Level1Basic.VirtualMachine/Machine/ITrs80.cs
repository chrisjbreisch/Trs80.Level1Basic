using System;
using System.IO;

using Trs80.Level1Basic.HostMachine;

namespace Trs80.Level1Basic.VirtualMachine.Machine;

public interface ITrs80
{
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
    HostFont GetCurrentFont();
    void SetCurrentFont(HostFont font);
    ConsoleKeyInfo ReadKey();
    void InitializeWindow();
}