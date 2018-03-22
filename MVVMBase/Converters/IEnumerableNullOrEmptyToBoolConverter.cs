using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace nkristek.MVVMBase.Converters
{
    /// <summary>
    /// Expects <see cref="IEnumerable{T}"/>.
    /// Returns true if it is null or empty.
    /// </summary>
    public class IEnumerableNullOrEmptyToBoolConverter
        : IValueConverter
    {
        public static IEnumerableNullOrEmptyToBoolConverter Instance = new IEnumerableNullOrEmptyToBoolConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is IEnumerable<object>) || !(value as IEnumerable<object>).Any();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
