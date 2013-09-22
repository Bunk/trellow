using Strilanc.Value;

namespace trello.Extensions
{
    public static class MayExtensions
    {
        public static May<T> MayCast<T>(this object obj) where T : class
        {
            var o = obj as T;
            return o == null ? May<T>.NoValue : new May<T>(o);
        }
    }
}