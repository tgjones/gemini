using Gemini.Framework.Util;
using Gemini.Framework.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Gemini.Framework.Controls
{
    public abstract partial class AdvancedSliderBase : UserControl, IDisposable
    {
        static AdvancedSliderBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AdvancedSliderBase), new FrameworkPropertyMetadata(typeof(AdvancedSliderBase)));
        }

        public AdvancedSliderBase()
        {
            _clickTimer = new System.Timers.Timer(NativeMethods.GetDoubleClickTime());
            _clickTimer.Elapsed += ClickTimer_Elapsed;
            InitializeComponent();
        }

        private void Update()
        {
            numberText.Text = DisplayText;

            if (Type == DisplayType.Number)
            {
                bar.Visibility = Visibility.Hidden;
                return;
            }

            var width = grid.ActualWidth;

            switch (Type)
            {
                case DisplayType.Bar:
                    bar.Visibility = Visibility.Visible;
                    bar.HorizontalAlignment = HorizontalAlignment.Left;
                    bar.Margin = new Thickness(0.0);
                    bar.Width = width * Ratio;
                    break;
                case DisplayType.Line:
                    if (Ratio == 0.5)
                    {
                        bar.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        bar.Visibility = Visibility.Visible;

                        var hwidth = (width / 2.0);
                        if (Ratio > 0.5)
                        {
                            bar.HorizontalAlignment = HorizontalAlignment.Left;
                            bar.Width = Math.Ceiling(hwidth * ((Ratio - 0.5) * 2.0));
                            bar.Margin = new Thickness(Math.Floor(hwidth), 0.0, 0.0, 0.0);
                        }
                        else
                        {
                            bar.HorizontalAlignment = HorizontalAlignment.Right;
                            bar.Width = Math.Ceiling(hwidth * (1.0 - (Ratio * 2.0)));
                            bar.Margin = new Thickness(0.0, 0.0, Math.Floor(hwidth), 0.0);
                        }
                    }
                    break;
            }
        }

        public string DisplayText
        {
            get { return (string) GetValue(DisplayTextProperty); }
            set { SetValue(DisplayTextProperty, value); }
        }

        public static readonly DependencyProperty DisplayTextProperty =
            DependencyProperty.Register("DisplayText", typeof(string), typeof(AdvancedSliderBase),
                new FrameworkPropertyMetadata(string.Empty, DependencyPropertyChanged)
                );

        public string EditText
        {
            get { return (string) GetValue(EditTextProperty); }
            set { SetValue(EditTextProperty, value); }
        }

        public static readonly DependencyProperty EditTextProperty =
            DependencyProperty.Register("EditText", typeof(string), typeof(AdvancedSliderBase),
                new FrameworkPropertyMetadata(string.Empty)
                );

        public enum DisplayType
        {
            Number,
            Bar,
            Line
        }

        public DisplayType Type
        {
            get { return (DisplayType) GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(DisplayType), typeof(AdvancedSliderBase),
                new FrameworkPropertyMetadata(DisplayType.Number, DependencyPropertyChanged)
                );

        public double Ratio
        {
            get { return (double) GetValue(RatioProperty); }
            set { SetValue(RatioProperty, value); }
        }

        public static readonly DependencyProperty RatioProperty =
            DependencyProperty.Register("Ratio", typeof(double), typeof(AdvancedSliderBase),
                new FrameworkPropertyMetadata(0.5, DependencyPropertyChanged, CoerceValue)
                );

        public Brush BarBrush
        {
            get { return (Brush) GetValue(BarBrushProperty); }
            set { SetValue(BarBrushProperty, value); }
        }

        public static readonly DependencyProperty BarBrushProperty =
            DependencyProperty.Register("BarBrush", typeof(Brush), typeof(AdvancedSliderBase),
                new FrameworkPropertyMetadata(Brushes.DarkGray, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, DependencyPropertyChanged)
                );

        private static void DependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = d as AdvancedSliderBase;
            slider.Update();
        }

        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            var value = (double) baseValue;
            if (value < 0.0)
                value = 0.0;
            if (value > 1.0)
                value = 1.0;
            return value;
        }

        public bool MouseCaptured
        {
            get { return (bool) GetValue(MouseCapturedProperty); }
            set { SetValue(MouseCapturedProperty, value); }
        }

        public static readonly DependencyProperty MouseCapturedProperty =
            DependencyProperty.Register("MouseCaptured", typeof(bool), typeof(AdvancedSliderBase));

        private double _startValue;
        private Point _originalPos;
        private Point _resetPos;
        private Point _lastPos;
        private bool _lostMouseGuard;
        private bool _relativeMouse;

        private System.Timers.Timer _clickTimer;
        private DateTime _firstClickTime;
        private int _clickCounter;

        private void ClickTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _clickTimer.Stop();
            _clickCounter = 0;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (e.ChangedButton == MouseButton.Right)
            {
                /* User aborted */
                EndMouseCapture(true);
                return;
            }

            if (_clickCounter > 0)
            {
                /* Double click, enable editor */
                BeginTextBoxUpdate();
                return;
            }

            MouseCaptured = CaptureMouse();
            if (!MouseCaptured)
                return;

            _startValue = Ratio;

            _lastPos = _originalPos = PointToScreen(Mouse.GetPosition(this));
            Mouse.OverrideCursor = Cursors.None;
            _resetPos = new Point(SystemParameters.PrimaryScreenWidth / 2, SystemParameters.PrimaryScreenHeight / 2);
            _relativeMouse = NativeMethods.SetCursorPos(_resetPos);

            _clickTimer.Stop();
            _clickCounter++;
            _clickTimer.Start();
            _firstClickTime = DateTime.Now;
        }

        private void EndMouseCapture(bool reset)
        {
            try
            {
                _lostMouseGuard = true;
                if (IsMouseCaptured)
                    ReleaseMouseCapture();
            }
            finally
            {
                _lostMouseGuard = false;
            }

            if (!MouseCaptured)
                return;

            /* Reset if user only pressed very short */
            if ((DateTime.Now - _firstClickTime).TotalMilliseconds < 100)
                reset = true;

            if (reset)
                Ratio = _startValue;

            MouseCaptured = false;
            Mouse.OverrideCursor = null;
            NativeMethods.SetCursorPos(_originalPos);
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            if (!_lostMouseGuard)
                EndMouseCapture(false);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            EndMouseCapture(false);
        }

        private bool _failing = false;

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!MouseCaptured)
                return;

            e.Handled = true;

            var pos = PointToScreen(e.GetPosition(this));
            var diff = (pos - (_relativeMouse ? _resetPos : _lastPos)).X;
            var multi = SpeedMultiplier.Get();

            ApplyValueChange(multi * diff);

            /*
             * TODO: There seems to be a bug with Synergy where setting the cursor position can fail.
             * https://github.com/symless/synergy/issues/5372
             */
            _relativeMouse = NativeMethods.SetCursorPos(_resetPos);

            _lastPos = pos;
        }

        public abstract void ApplyValueChange(double ammount);

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Update();
        }

        protected abstract void CommitEditText();

        private void BeginTextBoxUpdate()
        {
            textBox.Text = EditText;
            textBox.Visibility = Visibility.Visible;
            numberText.Visibility = Visibility.Hidden;
            textBox.SelectAll();
            textBox.Focus();
        }

        private void EndTextBoxUpdate(bool commit)
        {
            if (commit)
            {
                EditText = textBox.Text;
                CommitEditText();
            }

            textBox.Visibility = Visibility.Hidden;
            numberText.Visibility = Visibility.Visible;
        }

        private void numberText_GotFocus(object sender, RoutedEventArgs e)
        {
            BeginTextBoxUpdate();
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            EndTextBoxUpdate(true);
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                EndTextBoxUpdate(false);
            }
            else if (e.Key == Key.Escape)
            {
                e.Handled = true;
                EndTextBoxUpdate(true);
            }
        }

        #region IDisposable Support

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_clickTimer != null)
                    {
                        _clickTimer.Dispose();
                        _clickTimer = null;
                    }
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
