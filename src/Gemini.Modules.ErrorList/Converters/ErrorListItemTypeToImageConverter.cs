using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Gemini.Modules.ErrorList.Converters
{
    public class ErrorListItemTypeToImageConverter : IValueConverter
    {
        public ImageSource ErrorImageSource { get; set; }
        public ImageSource MessageImageSource { get; set; }
        public ImageSource WarningImageSource { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ErrorListItemType) value)
            {
                case ErrorListItemType.Error:
                    return ErrorImageSource;
                case ErrorListItemType.Warning:
                    return WarningImageSource;
                case ErrorListItemType.Message:
                    return MessageImageSource;
                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}