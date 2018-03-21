using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace nkristek.MVVMBase.Converters
{
    /// <summary>
    /// Expects <see cref="object"/>.
    /// Returns <see cref="Visibility.Visible"/> if <see cref="object.ToString"/> equals the given parameter.
    /// Returns <see cref="Visibility.Collapsed"/> otherwise.
    /// </summary>
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
