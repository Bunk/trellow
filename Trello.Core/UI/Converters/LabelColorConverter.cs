using System;
using System.Globalization;
using System.Windows.Data;

namespace Trellow.UI.Converters
{
    public class LabelColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = value.ToString().ToLowerInvariant();
            switch (color)
            {
                case "red":
                    return "#ffcb4d4d";
                case "green":
                    return "#ff34b27d";
                case "blue":
                    return "#ff4d77cb";
                case "yellow":
                    return "#ffdbdb57";
                case "purple":
                    return "#ff9933cc";
                case "orange":
                    return "#ffe09952";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
