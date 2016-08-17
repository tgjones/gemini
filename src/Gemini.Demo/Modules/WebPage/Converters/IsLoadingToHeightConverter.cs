using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Gemini.Demo.Modules.WebPage.Converters
{
    public class IsLoadingToHeightConverter : IValueConverter
    {
        double minHeight = 1;
        double normalHeight = 4;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return minHeight;
            else
                return (bool)value ? normalHeight : minHeight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
