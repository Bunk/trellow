using Strilanc.Value;

namespace Trellow
{
    public static class MayExtensions
    {
        public delegate bool TryParseHandler<T>(string value, out T result);

        public static May<T> MayParse<T>(this string str, TryParseHandler<T> handler)
        {
            if (string.IsNullOrWhiteSpace(str))
                return May<T>.NoValue;

            T parsed;
            return handler(str, out parsed) ? parsed : May<T>.NoValue;
        }

        public static May<T> MayCast<T>(this object obj) where T : class
        {
            var o = obj as T;
            return o == null ? May<T>.NoValue : new May<T>(o);
        }
    }
}