using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Gemini.Modules.Inspector.Util;
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

        public event EventHandler PickingStarted;
        public event EventHandler PickingCancelled;
        public event EventHandler<ColorEventArgs> ColorHovered;
        public event EventHandler<ColorEventArgs> ColorPicked;

        private readonly DispatcherTimer _timer;
        private Bitmap _bitmap;
        private bool _endingCapture;

        public ScreenColorPicker()
        {
            _timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 50)
            };
            _timer.Tick += OnTimerTick;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (IsMouseCaptured)
                RaiseColorHovered(GetEventArgs());
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                ReleaseMouseCapture();
            base.OnPreviewKeyDown(e);
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            _timer.Stop();
            _bitmap.Dispose();
            _bitmap = null;

            if (!_endingCapture)
                RaisePickingCancelled(EventArgs.Empty);

            base.OnLostMouseCapture(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (!IsMouseCaptured)
            {
                if (_bitmap != null)
                    _bitmap.Dispose();
                _bitmap = ScreenShotUtility.Take();

                if (Focus()) // So that we get the Escape key.
                {
                    if (CaptureMouse())
                    {
                        RaisePickingStarted(EventArgs.Empty);
                        _timer.Start();
                    }
                }
            }
            else
            {
                RaiseColorPicked(GetEventArgs());

                _endingCapture = true;
                ReleaseMouseCapture();
                _endingCapture = false;
            }

            base.OnMouseDown(e);
        }

        private ColorEventArgs GetEventArgs()
        {
            NativeMethods.NativePoint cursorPosition;
            if (NativeMethods.GetCursorPos(out cursorPosition))
            {
                cursorPosition.X -= (int) SystemParameters.VirtualScreenLeft;
                cursorPosition.Y -= (int) SystemParameters.VirtualScreenTop;

                if (cursorPosition.X > 0 && cursorPosition.X < _bitmap.Width &&
                    cursorPosition.Y > 0 && cursorPosition.Y < _bitmap.Height)
                {
                    var pixel = _bitmap.GetPixel(cursorPosition.X, cursorPosition.Y);
                    return new ColorEventArgs(Color.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B));
                }
            }
            return new ColorEventArgs(Colors.Black);
        }

        private void RaisePickingStarted(EventArgs e)
        {
            var handler = PickingStarted;
            if (handler != null) handler(this, e);
        }

        private void RaisePickingCancelled(EventArgs e)
        {
            var handler = PickingCancelled;
            if (handler != null) handler(this, e);
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