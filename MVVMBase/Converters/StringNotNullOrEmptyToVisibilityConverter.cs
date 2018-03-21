using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace nkristek.MVVMBase.Converters
{
    /// <summary>
    /// Expects <see cref="string"/>.
    /// Returns <see cref="Visibility.Visible"/> if it is not null or empty.
    /// Returns <see cref="Visibility.Hidden"/> if it is null or empty "Hidden" was set as a parameter.
    /// Returns <see cref="Visibility.Collapsed"/> otherwise.
    /// </summary>
    public class StringNotNullOrEmptyToVisibilityConverter
        : IValueConverter
    {
        public static StringNotNullOrEmptyToVisibilityConverter Instance = new StringNotNullOrEmptyToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!String.IsNullOrEmpty(value as string))
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
