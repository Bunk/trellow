using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace trello.Views.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = false;
            if (parameter != null)
                bool.TryParse(parameter.ToString(), out invert);

            var visible = true;
            if (value is bool)
                visible = (bool)value;
            else if (value is int)
                visible = (int)value != 0;
            else if (value is string)
                visible = !string.IsNullOrWhiteSpace((string)value);
            else if (value == null)
                visible = false;

            if (invert)
                visible = !visible;

            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}