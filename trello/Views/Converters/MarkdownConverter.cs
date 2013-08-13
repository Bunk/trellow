using System;
using System.Globalization;
using System.Windows.Data;
using Farse.Markdown;

namespace trello.Views.Converters
{
    public class MarkdownConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return HtmlTransform.Transform((string) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
