using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;

namespace trellow.api.Data.Services
{
    public static class RestClientExtensions
    {
        public static Task<IRestResponse> ExecuteAwaitable(this IRestClient client, IRestRequest request)
        {
            var completionSource = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, (response, handle) =>
            {
                if (response.ErrorException != null)
                    completionSource.SetException(new TrelloRequestException(response.Content, response.ErrorException));
                else
                    completionSource.SetResult(response);
            });
            return completionSource.Task;
        }

        public static Task<IRestResponse<T>> ExecuteAwaitable<T>(this IRestClient client, IRestRequest request)
        {
            var completionSource = new TaskCompletionSource<IRestResponse<T>>();
            client.ExecuteAsync<T>(request, (response, handle) =>
            {
                if (response.ErrorException != null)
                    completionSource.SetException(new TrelloRequestException(response.Content, response.ErrorException));
                else
                    completionSource.SetResult(response);
            });
            return completionSource.Task;
        }

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