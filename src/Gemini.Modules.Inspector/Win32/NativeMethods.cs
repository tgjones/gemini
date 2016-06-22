using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;

namespace Gemini.Modules.Inspector.Win32
{
    internal static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NativePoint
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
        public static extern bool GetCursorPos(out NativePoint point);
    }
}