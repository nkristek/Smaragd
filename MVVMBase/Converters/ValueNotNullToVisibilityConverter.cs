using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace nkristek.MVVMBase.Converters
{
    /// <summary>
    /// Expects <see cref="object"/>.
    /// Returns <see cref="Visibility.Visible"/> if it is not null.
    /// Returns <see cref="Visibility.Hidden"/> if it is null and "Hidden" was set as a parameter.
    /// Returns <see cref="Visibility.Collapsed"/> otherwise.
    /// </summary>
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
