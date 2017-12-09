using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace nkristek.MVVMBase.Converters
{
    public class AnyBoolToBoolConverter
        : IMultiValueConverter
    {
        public static AnyBoolToBoolConverter Instance = new AnyBoolToBoolConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Any(v => v is bool && (bool)v);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
