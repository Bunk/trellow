using System;
using System.Globalization;
using System.Windows.Data;

namespace trello.Views.Converters
{
    public class CaseConverter : IValueConverter 
    {
        public bool Uppercase { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return Uppercase ? value.ToString().ToUpperInvariant() : value.ToString().ToLowerInvariant();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
