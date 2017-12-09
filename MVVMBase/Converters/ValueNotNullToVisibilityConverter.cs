using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace nkristek.MVVMBase.Converters
{
    public class ValueNotNullToVisibilityConverter
        : IValueConverter
    {
        public static ValueNotNullToVisibilityConverter Instance = new ValueNotNullToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return Visibility.Visible;

            switch (parameter as string)
            {
                case "Hidden": return Visibility.Hidden;
                default: return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
