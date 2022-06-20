using Trs80.Level1Basic.HostMachine;

namespace Trs80.Level1Basic.TestUtilities
{
    public class FakeHost : IHost
    {
        private const int ScreenWidth = 64;
        private const int ScreenHeight = 16;
        private const int ScreenPixelWidth = 2 * ScreenWidth;
        private const int ScreenPixelHeight = 3 * ScreenHeight;
        private readonly bool[,] _screen = new bool[ScreenPixelWidth, ScreenPixelHeight];
        private int _cursorX;
        private int _cursorY;

        public FakeHost()
        {
            ClearScreen();
        }

        private void ClearScreen()
        {
            Erase(0, 0, ScreenPixelWidth, ScreenPixelHeight);
            _cursorX = 0;
            _cursorY = 0;
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
            ClearScreen();
        }

        public ConsoleKeyInfo ReadKey()
        {
            return new ConsoleKeyInfo();
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
            Out.WriteLine(text);
            _cursorX = 0;
            _cursorY++;
        }

        public void Write(string text)
        {
            Out.Write(text);
            _cursorX += text.Length;
        }

        public void Write(char c)
        {
            Out.Write(c);
            _cursorX++;
        }

        public string ReadLine()
        {
            return In.ReadLine()!.ToUpperInvariant();
        }

        public string GetFileNameForSave()
        {
            return string.Empty;
        }

        public string GetFileNameForLoad()
        {
            return string.Empty;
        }
    }
}
