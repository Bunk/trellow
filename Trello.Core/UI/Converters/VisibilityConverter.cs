using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Trellow.UI.Converters
{
    public class VisibilityConverter : IValueConverter
    {
		public bool Invert { get; set; }
		
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var invert = false;
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
            else if (value is IEnumerable)
                visible = ((IEnumerable) value).Cast<object>().Any();

            if (Invert || invert)
                visible = !visible;

            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}