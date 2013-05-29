using System;
using System.Globalization;
using System.Windows.Data;

namespace Gemini.Modules.ErrorList.Converters
{
    public class PluralizingValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var number = (int) value;
            var text = (string) parameter;

            if (number == 1)
                return text;

            return text + "s";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}