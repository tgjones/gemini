using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gemini.Modules.Inspector.Controls
{
    public class NumericTextBox : Control
    {
        private TextBlock _textBlock;
        private TextBox _textBox;

        static NumericTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericTextBox),
                new FrameworkPropertyMetadata(typeof(NumericTextBox)));
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(double), typeof(NumericTextBox),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double Value
        {
            get { return (double) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(
            "Mode", typeof(NumericTextBoxMode), typeof(NumericTextBox),
            new FrameworkPropertyMetadata(NumericTextBoxMode.Normal));

        public NumericTextBoxMode Mode
        {
            get { return (NumericTextBoxMode) GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            _textBlock = (TextBlock) Template.FindName("TextBlock", this);

            var originalPosition = new System.Drawing.Point();
            var mouseMoved = false;

            _textBlock.MouseDown += (sender, e) =>
            {
                originalPosition = System.Windows.Forms.Cursor.Position;
                _textBlock.CaptureMouse();
                Mouse.OverrideCursor = Cursors.None;
                mouseMoved = false;
            };

            _textBlock.MouseMove += (sender, e) =>
            {
                if (!_textBlock.IsMouseCaptured)
                    return;

                mouseMoved = true;

                var newPosition = System.Windows.Forms.Cursor.Position;
                Value += (newPosition.X - originalPosition.X) / 50.0;

                System.Windows.Forms.Cursor.Position = originalPosition;
            };

            _textBlock.MouseUp += (sender, e) =>
            {
                Mouse.OverrideCursor = null;
                _textBlock.ReleaseMouseCapture();

                if (!mouseMoved)
                {
                    Mode = NumericTextBoxMode.TextBox;
                    _textBox.Focus();
                }
            };

            _textBox = (TextBox) Template.FindName("TextBox", this);
            _textBox.KeyUp += (sender, e) =>
            {
                if (e.Key == Key.Escape || e.Key == Key.Enter)
                    Mode = NumericTextBoxMode.Normal;
            };
            _textBox.LostFocus += (sender, e) => Mode = NumericTextBoxMode.Normal;

            base.OnApplyTemplate();
        }
    }

    public enum NumericTextBoxMode
    {
        Normal,
        TextBox
    }
}