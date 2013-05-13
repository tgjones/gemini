using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.Xna.Framework;

namespace Gemini.Modules.Inspector.Xna.Converters
{
    public class XnaColorToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (Color) value;
            return System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (System.Windows.Media.Color) value;
            return Color.FromNonPremultiplied(c.R, c.G, c.B, c.A);
        }
    }
}