using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Caliburn.Micro;

namespace Gemini.Modules.UndoRedo.Converters
{
    public class HoveredIndexToBackgroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var items = (IObservableCollection<IUndoableAction>) values[0];
            var hoveredIndex = (int) values[1];
            var thisItem = (IUndoableAction) values[2];

            var thisItemIndex = items.IndexOf(thisItem);
            if (thisItemIndex <= hoveredIndex)
                return new SolidColorBrush(Colors.LightYellow);

            return new SolidColorBrush(Colors.Transparent);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}