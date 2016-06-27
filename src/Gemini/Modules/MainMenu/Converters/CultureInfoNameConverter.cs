using System;
using System.Globalization;
using System.Windows.Data;

namespace Gemini.Modules.MainMenu.Converters
{
    public class CultureInfoNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (string.Empty.Equals(value))
            {
                if (Properties.Resources.LanguageSystem.Equals("System"))
                    return Properties.Resources.LanguageSystem;

                return string.Format("{0} ({1})",
                    Properties.Resources.LanguageSystem,
                    Properties.Resources.ResourceManager.GetString("LanguageSystem", CultureInfo.InvariantCulture)
                    );
            }

            var cn = value as string;
            var ci = CultureInfo.GetCultureInfo(cn);

            if (Equals(ci.NativeName, ci.EnglishName))
                return ci.NativeName;

            return string.Format("{0} ({1})", ci.NativeName, ci.EnglishName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
