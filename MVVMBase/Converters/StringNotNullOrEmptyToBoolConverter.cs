using System;
using System.Globalization;
using System.Windows.Data;

namespace nkristek.MVVMBase.Converters
{
    public class StringNotNullOrEmptyToBoolConverter
        : IValueConverter
    {
        public static StringNotNullOrEmptyToBoolConverter Instance = new StringNotNullOrEmptyToBoolConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !String.IsNullOrEmpty(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
