using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;

namespace trello.Services
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

        public static Dictionary<string, string> ParseQueryString(this IRestResponse response)
        {
            var content = response.Content;
            if (content.Length > 0 && content[0] == '?')
                content = content.Substring(1);

            var retval = content.Split('&')
                .Select(pair => pair.Split('='))
                .ToDictionary(tuple => tuple[0], tuple => tuple.Length == 2 ? tuple[1] : string.Empty);

            return retval;
        }
    }
}