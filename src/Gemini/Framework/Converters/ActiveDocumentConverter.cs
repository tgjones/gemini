using System;
using System.Windows.Data;
using Caliburn.Micro;

namespace Gemini.Framework.Converters
{
	public class ActiveDocumentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is IScreen)
				return value;

			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is IScreen)
				return value;

			return Binding.DoNothing;
		}
	}
}