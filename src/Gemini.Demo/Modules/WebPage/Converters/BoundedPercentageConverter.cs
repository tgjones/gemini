using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Gemini.Demo.Modules.WebPage.Converters
{
    public class BoundedPercentageConverter : IValueConverter
    {
        double minValue = 10;
        double maxValue = 100;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return minValue;
            else
            {
                double val = Math.Min(maxValue, Math.Max(minValue, (double)value));
                return val;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
