using System.Collections.Generic;
using System.Linq;

namespace trello.Extensions
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> source,
                                                                   params Dictionary<TKey, TValue>[] dictionaries)
        {
            if (source == null)
                source = new Dictionary<TKey, TValue>();

            if (dictionaries == null)
                dictionaries = new Dictionary<TKey, TValue>[0];

            var list = new HashSet<Dictionary<TKey, TValue>>(dictionaries) {source};
            return list.SelectMany(dict => dict)
                       .ToLookup(pair => pair.Key, pair => pair.Value)
                       .ToDictionary(group => group.Key, group => group.First());
        }
    }
}
