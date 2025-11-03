using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Unibox.Converters
{
    internal class FilenameFromFullPath : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string fullPath = value as string;
            if (string.IsNullOrEmpty(fullPath)) return string.Empty;
            else return Path.Combine(Path.GetFileName(Path.GetDirectoryName(fullPath)), Path.GetFileName(fullPath)) ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}