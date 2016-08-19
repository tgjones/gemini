using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Gemini.Modules.Inspector.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; } = Visibility.Visible;

        public Visibility FalseValue { get; set; } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new NotSupportedException();

            bool visible = false;

            if (value is bool)
            {
                visible = (bool) value;
            }
            else if (value is bool?)
            {
                var nullableBool = (bool?) value;
                if (nullableBool.HasValue)
                    visible = nullableBool.Value;
            }
            else
            {
                throw new NotSupportedException();
            }

            return visible ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new NotSupportedException();

            if (Equals(value, TrueValue))
                return true;

            if (Equals(value, FalseValue))
                return false;

            throw new NotSupportedException();
        }
    }
}
