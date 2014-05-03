using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.Xna.Framework;

namespace Gemini.Modules.Inspector.MonoGame.Converters
{
    public class XnaColorToColorConverter : IValueConverter
    {
        public static System.Windows.Media.Color Convert(Color c)
        {
            return System.Windows.Media.Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((Color) value);
        }

        public static Color Convert(System.Windows.Media.Color c)
        {
            return Color.FromNonPremultiplied(c.R, c.G, c.B, c.A);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((System.Windows.Media.Color) value);
        }
    }
}