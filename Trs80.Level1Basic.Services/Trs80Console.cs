using System;
using System.IO;

namespace Trs80.Level1Basic.Services
{
    public interface ITrs80Console
    {
        TextWriter InternalWriter { get; set; }
        TextReader InternalReader { get; set; }
        void WriteLine(string text);
        void Write(string text);
        string ReadLine();
        void Clear();
        void SetCursorPosition(int row, int column);
    }

    public class Trs80Console : ITrs80Console, IDisposable
    {
        public TextWriter InternalWriter { get; set;  }
        public TextReader InternalReader { get; set; }

        public Trs80Console()
        {
            InternalWriter = Console.Out;
            InternalReader = Console.In;
        }

        public void WriteLine(int value)
        {
            InternalWriter.WriteLine(value);
        }

        public void WriteLine(bool value)
        {
            InternalWriter.WriteLine(value);
        }

        public void WriteLine(double value)
        {
            InternalWriter.WriteLine(value);
        }

        public void WriteLine(string text)
        {
            InternalWriter.WriteLine(text);
        }

        public void Write(int value)
        {
            InternalWriter.Write(value);
        }

        public void Write(bool value)
        {
            InternalWriter.Write(value);
        }

        public void Write(double value)
        {
            InternalWriter.Write(value);
        }

        public void Write(string text)
        {
            InternalWriter.Write(text);
        }

        public string ReadLine()
        {
            return InternalReader.ReadLine();
        }

        public void Clear()
        {
            Console.Clear();
        }

        public void SetCursorPosition(int row, int column)
        {
            Console.SetCursorPosition(row, column);
        }

        public void Dispose()
        {
            InternalWriter?.Dispose();
        }
    }
}
