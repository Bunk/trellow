using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace trello.Views.Converters
{
    public class DueDateColorConverter : IValueConverter
    {
        public SolidColorBrush DefaultBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DefaultBrush ?? new SolidColorBrush(FromHexa("#ff34b27d"));

            var strVal = value.ToString();
            var nullable = value as DateTime?;
            if (nullable != null)
                strVal = nullable.Value.ToString(CultureInfo.InvariantCulture);

            DateTime dateval;
            if (DateTime.TryParse(strVal, out dateval))
            {
                var given = dateval.ToLocalTime();
                var current = DateTime.Now;

                var difference = given - current;

                if (difference.Ticks < 0)
                    return new SolidColorBrush(FromHexa("#ffcb4d4d"));//FromHexa("#ffa82424"));

                if (difference.TotalHours <= 24)
                    return new SolidColorBrush(FromHexa("#ffdbdb57"));//FromHexa("#ffc3b622"));
            }
            return DefaultBrush ?? new SolidColorBrush(FromHexa("#ff34b27d"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private Color FromHexa(string hex)
        {
            return Color.FromArgb(
                System.Convert.ToByte(hex.Substring(1, 2), 16),
                System.Convert.ToByte(hex.Substring(3, 2), 16),
                System.Convert.ToByte(hex.Substring(5, 2), 16),
                System.Convert.ToByte(hex.Substring(7, 2), 16));
        }

        private Color FromKnownColor(string color)
        {
            return (Color) XamlReader.Load(
                @"<Color xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">"
                + color + "</Color>");
        }
    }
}
