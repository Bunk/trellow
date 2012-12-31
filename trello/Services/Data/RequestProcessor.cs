using System.Threading.Tasks;
using RestSharp;
using trello.Services.OAuth;

namespace trello.Services.Data
{
    public interface IRequestProcessor
    {
        Task<T> Execute<T>(IRestRequest request);
    }

    public class RequestProcessor : IRequestProcessor
    {
        private readonly IOAuthClient _factory;

        public RequestProcessor(IOAuthClient factory)
        {
            _factory = factory;
        }

        public async Task<T> Execute<T>(IRestRequest request)
        {
            if (!_factory.ValidateAccessToken())
                return default(T);

            OnQueryStart<T>(request);

            var client = _factory.GetRestClient();
            var response = await client.ExecuteAwaitable<T>(request);
            var data = response.Data;

            OnQueryComplete(request, data);

            return data;
        }

        protected virtual void OnQueryStart<T>(IRestRequest request)
        {
            
        }

        protected virtual void OnQueryComplete<T>(IRestRequest request, T result)
        {
            
        }
    }
}
