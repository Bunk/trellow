using System.Collections.Generic;
using System.Linq;

namespace trellow.api.Data.Services
{
    public static class RestClientExtensions
    {
        public static Dictionary<string, string> ParseQueryString(this string queryString)
        {
            if (queryString.Length > 0 && queryString[0] == '?')
                queryString = queryString.Substring(1);

            return queryString.Split('&')
                .Select(pair => pair.Split('='))
                .ToDictionary(tuple => tuple[0], tuple => tuple.Length == 2 ? tuple[1] : string.Empty);
        }
    }
}