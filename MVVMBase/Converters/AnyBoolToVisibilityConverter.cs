using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace nkristek.MVVMBase.Converters
{
    public class AnyBoolToVisibilityConverter
        : IMultiValueConverter
    {
        public static AnyBoolToVisibilityConverter Instance = new AnyBoolToVisibilityConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(v => v is bool && (bool)v))
                return Visibility.Visible;
            
            switch (parameter as string)
            {
                case "Hidden": return Visibility.Hidden;
                default: return Visibility.Collapsed;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
