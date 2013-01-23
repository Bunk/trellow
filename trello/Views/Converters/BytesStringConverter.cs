using System;
using System.Globalization;
using System.Windows.Data;

namespace trello.Views.Converters
{
    public class BytesStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            const string format = "#,0.0";
            double val;

            if (double.TryParse(value.ToString(), out val))
            {
                if (val < 1024)
                    return val.ToString("#,0", culture) + " bytes";

                val /= 1024;
                if (val < 1024)
                    return val.ToString(format, culture) + " KB"; // Kilobytes

                val /= 1024;
                if (val < 1024)
                    return val.ToString(format, culture) + " MB"; // Megabytes

                val /= 1024;
                if (val < 1024)
                    return val.ToString(format, culture) + " GB"; // Gigabytes
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}