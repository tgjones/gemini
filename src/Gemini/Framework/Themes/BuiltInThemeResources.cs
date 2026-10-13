using System.Windows;
using AvalonDock.Themes;

namespace Gemini.Framework.Themes
{
    internal static class BuiltInThemeResources
    {
        public static ResourceDictionary CreateAvalonDockDictionary(ITheme theme)
        {
            DictionaryTheme avalonDockTheme;
            var themeType = theme.GetType();

            if (themeType == typeof(LightTheme))
                avalonDockTheme = new Vs2013LightTheme();
            else if (themeType == typeof(DarkTheme))
                avalonDockTheme = new Vs2013DarkTheme();
            else if (themeType == typeof(BlueTheme))
                avalonDockTheme = new Vs2013BlueTheme();
            else
                return null;

            return avalonDockTheme.ThemeResourceDictionary;
        }
    }
}
