using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Gemini.Modules.Inspector.Controls
{
    [ContentProperty("Content")]
    public class LabelledControl : Control
    {
        static LabelledControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LabelledControl),
                new FrameworkPropertyMetadata(typeof(LabelledControl)));
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label", typeof(string), typeof(LabelledControl));

        public string Label
        {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof(UIElement), typeof(LabelledControl));

        public UIElement Content
        {
            get { return (UIElement) GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
    }
}