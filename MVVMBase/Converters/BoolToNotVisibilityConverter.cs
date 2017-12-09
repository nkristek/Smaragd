using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace nkristek.MVVMBase.Converters
{
    public class BoolToNotVisibilityConverter
        : IValueConverter
    {
        public static BoolToNotVisibilityConverter Instance = new BoolToNotVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && !(bool)value)
                return Visibility.Visible;

            switch (parameter as string)
            {
                case "Hidden": return Visibility.Hidden;
                default: return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility && ((Visibility)value == Visibility.Collapsed);
        }
    }
}
