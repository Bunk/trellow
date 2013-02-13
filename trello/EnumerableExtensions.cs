using System;
using System.Linq;
using System.Collections.Generic;

namespace trello
{
    public static class EnumerableExtensions
    {
        public static void DoAndClear<T>(this ICollection<T> coll, Action<T> act)
        {
            foreach (var item in coll)
                act(item);

            coll.Clear();
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> source, IEnumerable<T> compared, Func<T, object> keyExtractor)
        {
            return source.Except(compared, new KeyEqualityComparer<T>(keyExtractor));
        }
    }
}
