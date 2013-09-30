using System;
using System.Globalization;
using System.Windows.Data;

namespace Trellow.UI.Converters
{
    public class UriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Uri)
                return value.ToString();

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var uriString = value as string;
            return uriString != null ? new Uri(uriString, UriKind.Absolute) : value;
        }
    }
}