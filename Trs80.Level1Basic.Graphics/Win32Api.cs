using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Trs80.Level1Basic.Graphics
{
    public static class Win32Api
    {
        // https://www.pinvoke.net/default.aspx/kernel32/GetConsoleTitle.html
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint GetConsoleTitle([Out] StringBuilder lpConsoleTitle, uint nSize);

        // https://www.pinvoke.net/default.aspx/kernel32/SetConsoleTitle.html
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleTitle(string lpConsoleTitle);

        // https://www.pinvoke.net/default.aspx/user32/FindWindow.html
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindowFromWindowName(IntPtr zeroOnly, string lpWindowName);

        // https://www.pinvoke.net/default.aspx/user32/GetClientRect.html
        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);

        // https://www.pinvoke.net/default.aspx/user32/GetWindowRect.html
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out Rect lpRect);

        // https://www.pinvoke.net/default.aspx/user32/GetWindowInfo.html
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowInfo(IntPtr hwnd, ref WindowInfo pwi);


        // http://www.pinvoke.net/default.aspx/user32/GetParent.html
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);


        public static IntPtr GetConsoleWindowHandle()
        {
            var sb = new StringBuilder(1024);
            GetConsoleTitle(sb, 1024);

            string originalTitle = sb.ToString();

            string newTitle = originalTitle + Process.GetCurrentProcess().Id;
            SetConsoleTitle(newTitle);

            IntPtr hwnd;
            do
            {
                hwnd = FindWindowFromWindowName(IntPtr.Zero, newTitle);
            } while (!IsValidHwnd(hwnd));

            SetConsoleTitle(originalTitle);
            return hwnd;
        }

        private static bool IsValidHwnd(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero) return true;

            Thread.Sleep(100);
            return false;
        }

        public static Rect GetClientRect(IntPtr hWnd)
        {
            Rect result;
            GetClientRect(hWnd, out result);
            return result;
        }
        public static Rect GetWindowRect(IntPtr hWnd)
        {
            Rect result;
            GetWindowRect(hWnd, out result);
            return result;
        }

        public static WindowInfo GetWindowInfo(IntPtr hWnd)
        {
            WindowInfo info = new WindowInfo();
            info.cbSize = (uint)Marshal.SizeOf(info);
            GetWindowInfo(hWnd, ref info);
            return info;
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left;        // x position of upper-left corner
        public int Top;         // y position of upper-left corner
        public int Right;       // x position of lower-right corner
        public int Bottom;      // y position of lower-right corner
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WindowInfo
    {
        public uint cbSize;
        public Rect rcWindow;
        public Rect rcClient;
        public uint dwStyle;
        public uint dwExStyle;
        public uint dwWindowStatus;
        public uint cxWindowBorders;
        public uint cyWindowBorders;
        public ushort atomWindowType;
        public ushort wCreatorVersion;

        public WindowInfo(Boolean? filler) : this()   // Allows automatic initialization of "cbSize" with "new WINDOWINFO(null/true/false)".
        {
            cbSize = (UInt32)(Marshal.SizeOf(typeof(WindowInfo)));
        }

    }
}