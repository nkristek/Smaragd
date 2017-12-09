using System;
using System.Globalization;
using System.Windows.Data;

namespace nkristek.MVVMBase.Converters
{
    public class ValueNullToBoolConverter
        : IValueConverter
    {
        public static ValueNullToBoolConverter Instance = new ValueNullToBoolConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
