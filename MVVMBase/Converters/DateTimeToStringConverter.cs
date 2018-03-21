using System;
using System.Globalization;
using System.Windows.Data;

namespace nkristek.MVVMBase.Converters
{
    /// <summary>
    /// Expects a <see cref="DateTime"/>.
    /// Returns <see cref="string"/> representation. 
    /// Optionally a parameter can be set which will be used as a parameter of the <see cref="DateTime.ToString(string)"/> method.
    /// </summary>
    public class DateTimeToStringConverter
        : IValueConverter
    {
        public static DateTimeToStringConverter Instance = new DateTimeToStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var dateTime = (DateTime)value;
            if (dateTime == DateTime.MinValue)
                return null;

            if (parameter is string && !String.IsNullOrEmpty(parameter as string))
                return dateTime.ToString(parameter as string);

            return dateTime.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
