using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Unibox.Data.Enums;

namespace Unibox.Converters
{
    internal class UpdatePlatformMessageTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((UpdatePlatformMessageType)value)
            {
                case UpdatePlatformMessageType.Error:
                    return "🚫";

                case UpdatePlatformMessageType.Warning:
                    return "⚠️";

                case UpdatePlatformMessageType.Information:
                    return "";

                case UpdatePlatformMessageType.Success:
                    return "✔️";

                default:
                    return "Unknown";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}