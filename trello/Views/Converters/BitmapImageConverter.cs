using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace trello.Views.Converters
{
    public class BitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return new BitmapImage(new Uri(value.ToString(), UriKind.Absolute))
            {
                CreateOptions = CreateOptions
            };
        }

        public BitmapCreateOptions CreateOptions
        {
            get;
            set;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
