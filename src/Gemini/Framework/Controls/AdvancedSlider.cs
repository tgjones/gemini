using System;
using System.ComponentModel;
using System.Windows;

namespace Gemini.Framework.Controls
{
    public class AdvancedSlider : AdvancedSliderBase
    {
        public override void ApplyValueChange(double ammount)
        {
            var s = (dynamic) Speed;
            var change = s * ammount;

            var newValue = (dynamic) Value + Convert.ChangeType(change, ValueType);
            newValue = CoerceValue(this, newValue);

            Value = newValue;
        }

        protected override void CommitEditText()
        {
            try
            {

                Value = TypeDescriptor.GetConverter(ValueType).ConvertFrom(EditText);
            }
            catch (Exception ex)
            {
                var msg = string.Format(Properties.Resources.AdvancedSliderCommitErrorFormat, ex.Message);
                MessageBox.Show(Application.Current.MainWindow, msg, Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public Type ValueType
        {
            get { return (Type) GetValue(ValueTypeProperty); }
            set { SetValue(ValueTypeProperty, value); }
        }

        public static readonly DependencyProperty ValueTypeProperty =
            DependencyProperty.Register("ValueType", typeof(Type), typeof(AdvancedSlider));

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
                if (ValueType == null)
                    ValueType = Value.GetType();
            }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(AdvancedSlider),
                new FrameworkPropertyMetadata(DependencyPropertyChanged, CoerceValue)
                );

        public object ValueMin
        {
            get { return GetValue(ValueMinProperty); }
            set { SetValue(ValueMinProperty, value); }
        }

        public static readonly DependencyProperty ValueMinProperty =
            DependencyProperty.Register("ValueMin", typeof(object), typeof(AdvancedSlider),
                new FrameworkPropertyMetadata(DependencyPropertyChanged)
                );

        public object ValueMax
        {
            get { return GetValue(ValueMaxProperty); }
            set { SetValue(ValueMaxProperty, value); }
        }

        public static readonly DependencyProperty ValueMaxProperty =
            DependencyProperty.Register("ValueMax", typeof(object), typeof(AdvancedSlider),
                new FrameworkPropertyMetadata(DependencyPropertyChanged)
                );

        public object Speed
        {
            get { return GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }

        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.Register("Speed", typeof(object), typeof(AdvancedSlider),
                new FrameworkPropertyMetadata(DependencyPropertyChanged)
                );

        public string ValueFormat
        {
            get { return (string) GetValue(ValueFormatProperty); }
            set { SetValue(ValueFormatProperty, value); }
        }

        public static readonly DependencyProperty ValueFormatProperty =
            DependencyProperty.Register("ValueFormat", typeof(string), typeof(AdvancedSlider),
                new FrameworkPropertyMetadata("{0:0.#####}", DependencyPropertyChanged)
                );

        public string ValueEditFormat
        {
            get { return (string) GetValue(ValueEditFormatProperty); }
            set { SetValue(ValueEditFormatProperty, value); }
        }

        public static readonly DependencyProperty ValueEditFormatProperty =
            DependencyProperty.Register("ValueEditFormat", typeof(string), typeof(AdvancedSlider),
                new FrameworkPropertyMetadata("{0:0.#####}", DependencyPropertyChanged)
                );

        private static void DependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = d as AdvancedSlider;
            slider.UpdateValues();
        }

        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            if (baseValue == null)
                return null;

            var slider = d as AdvancedSlider;

            if (slider.Type == DisplayType.Number)
                return baseValue;

            var value = (dynamic) baseValue;

            if (slider.ValueMin != null)
            {
                if (value.CompareTo(slider.ValueMin) < 0)
                    value = slider.ValueMin;
            }

            if (slider.ValueMax != null)
            {
                if (value.CompareTo(slider.ValueMax) > 0)
                    value = slider.ValueMax;
            }

            return value;
        }

        private void UpdateValues()
        {
            if (Value == null)
                return;

            DisplayText = string.Format(ValueFormat, Value);
            EditText = string.Format(ValueEditFormat, Value);

            if (Type == DisplayType.Number)
                return;

            if (ValueMin != null && ValueMax != null)
            {
                try
                {
                    var v = (dynamic) Value;
                    var vmin = (dynamic) ValueMin;
                    var vmax = (dynamic) ValueMax;
                    Ratio = (v - vmin) / (vmax - vmin);
                }
                catch (DivideByZeroException)
                {
                }
            }
        }
    }
}
