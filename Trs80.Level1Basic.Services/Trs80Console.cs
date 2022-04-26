using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Trs80.Level1Basic.Graphics;

namespace Trs80.Level1Basic.Services
{
    public interface ITrs80Console
    {
        TextWriter Out { get; set; }
        TextReader In { get; set; }
        TextWriter Error { get; set; }
        void WriteLine(string text = "");
        void Write(string text);
        string ReadLine();
        void Clear();
        void SetCursorPosition(int row, int column);
        void SetCurrentFont(string font, short fontSize = 0);
        ConsoleKeyInfo ReadKey();
        void SetWindowSize(int width, int height);
        void SetBufferSize(int width, int height);
        void DisableCursorBlink();
    }

    public class Trs80Console : ITrs80Console, IDisposable
    {
        public TextWriter Out { get; set;  }
        public TextReader In { get; set; }
        public TextWriter Error { get; set; }

        public Trs80Console()
        {
            Out = Console.Out;
            In = Console.In;
            Error = Console.Error;
        }

        public void WriteLine(string text = "") => Out.WriteLine(text);

        public void Write(string text) => Out.Write(text);

        public string ReadLine() => In.ReadLine();

        public void Clear() => Console.Clear();

        public void SetCursorPosition(int row, int column) => Console.SetCursorPosition(row, column);

        private const uint EnableVirtualTerminalProcessing = 4;
        private const int FixedWidthTrueType = 54;
        private const int StandardOutputHandle = -11;

        private static readonly IntPtr ConsoleOutputHandle = Win32Api.GetStdHandle(StandardOutputHandle);

        public void SetCurrentFont(string font, short fontSize = 0)
        {
            var before = new Win32Api.FontInfo
            {
                cbSize = Marshal.SizeOf<Win32Api.FontInfo>()
            };

            if (Win32Api.GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref before))
            {
                var set = new Win32Api.FontInfo
                {
                    cbSize = Marshal.SizeOf<Win32Api.FontInfo>(),
                    FontIndex = 0,
                    FontFamily = FixedWidthTrueType,
                    FontName = font,
                    FontWeight = 400,
                    FontSize = fontSize > 0 ? fontSize : before.FontSize
                };

                // Get some settings from current font.
                if (!Win32Api.SetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref set))
                {
                    int ex = Marshal.GetLastWin32Error();
                    throw new Win32Exception(ex);
                }

                var after = new Win32Api.FontInfo
                {
                    cbSize = Marshal.SizeOf<Win32Api.FontInfo>()
                };
                Win32Api.GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref after);
            }
            else
            {
                int er = Marshal.GetLastWin32Error();
                throw new Win32Exception(er);
            }
        }

        public ConsoleKeyInfo ReadKey() => Console.ReadKey();

        public void SetWindowSize(int width, int height)
        {
            if (OperatingSystem.IsWindows())
                Console.SetWindowSize(width, height);
        }

        public void SetBufferSize(int width, int height)
        {
            if (OperatingSystem.IsWindows())
                Console.SetBufferSize(width, height);
        }
        
        public void DisableCursorBlink()
        {
            if (!Win32Api.GetConsoleMode(ConsoleOutputHandle, out uint lpMode))
            {
                int ex = Marshal.GetLastWin32Error();
                throw new Win32Exception(ex);
            }

            lpMode |= EnableVirtualTerminalProcessing;
            if (!Win32Api.SetConsoleMode(ConsoleOutputHandle, lpMode))
            {
                int ex = Marshal.GetLastWin32Error();
                throw new Win32Exception(ex);
            }
            Out.Write("\u001b[?12l");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            Out?.Dispose();
            In?.Dispose();
            Error?.Dispose();

            Out = null;
            In = null;
            Error = null;
        }
    }
}
