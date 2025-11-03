using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Unibox.Converters
{
    internal class CountToBoolConverter : IValueConverter
    {
        // Returns true when count >= parameter
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int i) return i >= int.Parse((string)parameter);
            if (value is long l) return l >= long.Parse((string)parameter);
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}