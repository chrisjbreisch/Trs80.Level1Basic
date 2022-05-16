using System;
using System.IO;

namespace Trs80.Level1Basic.HostMachine;

public interface IHost
{
    void EnableVirtualTerminal();
    HostFont GetCurrentConsoleFont();
    void SetCurrentConsoleFont(HostFont font);
    (int Left, int Top) GetCursorPosition();
    void SetCursorPosition(int column, int row);
    void Clear();
    ConsoleKeyInfo ReadKey();
    void SetWindowSize(int width, int height);
    void SetBufferSize(int width, int height);
    void Fill(int x, int y, int width, int height);
    void Erase(int x, int y, int width, int height);
    TextWriter Out { get; set; }
    TextReader In { get; set; }
    TextWriter Error { get; set; }
    void WriteLine(string text = "");
    void Write(string text);
    string ReadLine();
    string GetFileNameForSave();
    string GetFileNameForLoad();
}
