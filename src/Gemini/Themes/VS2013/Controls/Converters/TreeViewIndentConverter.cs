using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Gemini.Themes.VS2013.Controls.Converters
{
    public class TreeViewIndentConverter : IValueConverter
    {
        public double Indent { get; set; }

        private static int GetItemDepth(TreeViewItem item)
        {
            DependencyObject target = item;

            var depth = 0;
            do
            {
                if (target is TreeView)
                    return depth - 1;
                if (target is TreeViewItem)
                    depth++;
            } while ((target = VisualTreeHelper.GetParent(target)) != null);

            return 0;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as TreeViewItem;
            if (item == null)
                return new Thickness(0);

            return new Thickness(Indent * GetItemDepth(item), 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
