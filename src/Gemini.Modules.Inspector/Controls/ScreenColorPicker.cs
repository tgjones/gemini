using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gemini.Modules.Inspector.Win32;
using Color = System.Windows.Media.Color;

namespace Gemini.Modules.Inspector.Controls
{
    public class ScreenColorPicker : Control
    {
        static ScreenColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScreenColorPicker),
                new FrameworkPropertyMetadata(typeof(ScreenColorPicker)));
        }

        public event EventHandler<ColorEventArgs> ColorHovered;
        public event EventHandler<ColorEventArgs> ColorPicked;

        private Bitmap _bitmap;

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (!IsMouseCaptured)
            {
                if (_bitmap != null)
                    _bitmap.Dispose();
                _bitmap = NativeMethods.GetDesktop();

                CaptureMouse();
            }
            else
            {
                RaiseColorPicked(GetEventArgs());
                _bitmap.Dispose();
                ReleaseMouseCapture();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseCaptured)
                RaiseColorHovered(GetEventArgs());

            base.OnMouseMove(e);
        }

        private ColorEventArgs GetEventArgs()
        {
            var cursorPosition = System.Windows.Forms.Cursor.Position;
            var pixel = _bitmap.GetPixel(cursorPosition.X, cursorPosition.Y);
            return new ColorEventArgs(Color.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B));
        }

        private void RaiseColorHovered(ColorEventArgs e)
        {
            var handler = ColorHovered;
            if (handler != null) handler(this, e);
        }

        private void RaiseColorPicked(ColorEventArgs e)
        {
            var handler = ColorPicked;
            if (handler != null) handler(this, e);
        }
    }

    public class ColorEventArgs : EventArgs
    {
        public Color Color { get; private set; }

        public ColorEventArgs(Color color)
        {
            Color = color;
        }
    }
}