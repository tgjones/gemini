using System;
using System.Runtime.InteropServices;

namespace Gemini.Framework.Win32
{
    internal static class NativeMethods
    {
        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;

        public const int WS_MAXIMIZEBOX = 0x10000;
        public const int WS_MINIMIZEBOX = 0x20000;

        public const int WS_EX_DLGMODALFRAME = 0x00000001;

        public const int SWP_NOSIZE = 0x0001;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOZORDER = 0x0004;
        public const int SWP_FRAMECHANGED = 0x0020;

        public const uint WM_SETICON = 0x0080;

        [DllImport("user32.dll")]
        public extern static int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public extern static int SetWindowLong(IntPtr hwnd, int index, int value);

        [DllImport("user32.dll")]
        public extern static bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, 
            int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hwnd, uint msg, 
            IntPtr wParam, IntPtr lParam);
    }
}