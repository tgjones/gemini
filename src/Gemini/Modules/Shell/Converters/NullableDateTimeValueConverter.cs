using System;
using System.Globalization;
using System.Windows.Data;

namespace Gemini.Modules.Shell.Converters
{
    public class NullableDateTimeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertToFromDateTime(value, targetType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertToFromDateTime(value, targetType);
        }

        private static object ConvertToFromDateTime(object value, Type targetType)
        {
            //Rules out the null use case.
            if (value == null) return null;

            var valueType = value.GetType();

            //Also rules out we're making a same-Type conversion.
            if (valueType == targetType) return value;

            //Following which we should always have a value to work with.
            if (targetType == typeof (TimeSpan?)
                && (valueType == typeof (DateTime)
                    || valueType == typeof (DateTime?)))
            {
                //Effectively dropping the Date component from the DateTime.
                return ((DateTime?) value).Value.TimeOfDay;
            }

            /* TODO: it would be great (I mean REALLY GREAT) to pass the base value(s) in via parameter(s)
             * TODO: HOWEVER, I'm not sure this is a (well-)known issue of WPF?
             * Otherwise, should be able to convert to/from DateTime/TimeSpan all day long. */
            //see: http://www.codeproject.com/Articles/456589/Bindable-Converter-Parameter
            if (targetType == typeof (DateTime?)
                && (valueType == typeof (TimeSpan)
                    || valueType == typeof (TimeSpan?)))
            {
                //In this instance, assuming Now as the base.
                return DateTime.UtcNow.Date + ((TimeSpan?) value).Value;
                //TODO: TBD: would be better to expose a Bindable base value, but I read that this is a (well-?)known limitation
            }

            return null;
        }
    }
}
