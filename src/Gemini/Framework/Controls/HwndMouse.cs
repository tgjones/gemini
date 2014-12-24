using System.Windows;
using Gemini.Framework.Win32;

namespace Gemini.Framework.Controls
{
    public static class HwndMouse
    {
        public static Point GetCursorPosition()
        {
            var point = new NativeMethods.NativePoint();
            NativeMethods.GetCursorPos(ref point);
            return new Point(point.X, point.Y);
        }

        public static void SetCursorPosition(Point point)
        {
            NativeMethods.SetCursorPos((int) point.X, (int) point.Y);
        }

        public static void ShowCursor()
        {
            NativeMethods.ShowCursor(true);
        }

        public static void HideCursor()
        {
            NativeMethods.ShowCursor(false);
        }
    }
}