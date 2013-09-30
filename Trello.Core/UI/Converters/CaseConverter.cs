using System;
using System.Globalization;
using System.Windows.Data;

namespace Trellow.UI.Converters
{
    public enum Casing
    {
        Lower,
        Upper
    }

    public class CaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var casing = Casing.Lower;
            if (parameter != null)
                casing = (Casing) Enum.Parse(typeof (Casing), parameter.ToString(), true);

            switch (casing)
            {
                case Casing.Lower:
                    return value.ToString().ToLowerInvariant();
                case Casing.Upper:
                    return value.ToString().ToUpperInvariant();
                default:
                    return value.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}