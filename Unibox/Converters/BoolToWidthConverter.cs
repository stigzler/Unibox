using System;
using System.Globalization;
using System.Windows.Data;

namespace Unibox.Converters
{
    public class BoolToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (bool)value ? 240 : 48;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => (int)value == 170;
    }
}