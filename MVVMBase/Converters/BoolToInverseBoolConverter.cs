using System;
using System.Globalization;
using System.Windows.Data;

namespace nkristek.MVVMBase.Converters
{
    public class BoolToInverseBoolConverter
        : IValueConverter
    {
        public static BoolToInverseBoolConverter Instance = new BoolToInverseBoolConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool ? !(bool)value : false; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool? !(bool)value : false;
        }
    }
}
