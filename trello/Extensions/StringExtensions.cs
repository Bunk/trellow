using System;

namespace trello.Extensions
{
    public static class StringExtensions
    {
        public static Uri ToUri(this string str)
        {
            return string.IsNullOrWhiteSpace(str) ? null : new Uri(str, UriKind.Absolute);
        }
    }
}