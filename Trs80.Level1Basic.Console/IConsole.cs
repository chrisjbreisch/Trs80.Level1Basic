﻿using System;
using System.IO;

namespace Trs80.Level1Basic.Console
{
    public interface IConsole
    {
        int CursorX { get; }
        int CursorY { get; }
        TextWriter Out { get; set; }
        TextReader In { get; set; }
        TextWriter Error { get; set; }
        void WriteLine(string text = "");
        void Write(string text);
        void Write(char c);
        string ReadLine();
        void Clear();
        void SetCursorPosition(int column, int row);
        (int Left, int Top) GetCursorPosition();
        ConsoleFont GetCurrentFont();
        void SetCurrentFont(ConsoleFont font);
        ConsoleKeyInfo ReadKey();
        void InitializeWindow();
        void Set(int x, int y);
        void Reset(int x, int y);
        bool Point(int x, int y);
    }

}
