using System;
using System.Globalization;
using System.Windows.Data;
using Gemini.Framework;

namespace Gemini.Modules.Shell.Converters
{
	public class ActiveDocumentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is IDocument)
				return value;

			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is IDocument)
				return value;

			return Binding.DoNothing;
		}
	}
}