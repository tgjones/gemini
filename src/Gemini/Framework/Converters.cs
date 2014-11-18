using System.Windows.Data;
using Xceed.Wpf.AvalonDock.Converters;

namespace Gemini.Framework
{
    public static class Converters
    {
         public static readonly IValueConverter NullToDoNothingConverter = new NullToDoNothingConverter();
    }
}