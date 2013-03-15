using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;

namespace trellow.api
{
    public interface IRequestClient : IOAuth
    {
        Task<IRestResponse> RequestAsync(IRestRequest request);

        Task<T> RequestAsync<T>(IRestRequest request) where T : class, new();

        Task<IEnumerable<T>> RequestListAsync<T>(IRestRequest request);
    }
}