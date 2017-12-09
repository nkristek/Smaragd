using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace nkristek.MVVMBase.Converters
{
    public class ObjectToStringEqualsParameterToVisibilityConverter
        : IValueConverter
    {
        public static ObjectToStringEqualsParameterToVisibilityConverter Instance = new ObjectToStringEqualsParameterToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var parameterAsString = parameter as string;
            return value?.ToString() == parameterAsString ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
