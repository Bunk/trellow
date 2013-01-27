using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using trellow.api.OAuth;

namespace trellow.api.Data
{
    public interface IRequestProcessor
    {
        Task<T> Execute<T>(IRestRequest request);
    }

    public interface IHandleRequests
    {
        void BeforeRequest<T>(IRestRequest request);

        void AfterRequest<T>(IRestRequest request, T result);
    }

    public class RequestProcessor : IRequestProcessor
    {
        private readonly IOAuthClient _factory;

        private readonly IEnumerable<IHandleRequests> _handlers;

        public RequestProcessor(IOAuthClient factory, IEnumerable<IHandleRequests> requests)
        {
            _factory = factory;
            _handlers = requests ?? new List<IHandleRequests>();
        }

        public async Task<T> Execute<T>(IRestRequest request)
        {
            if (!_factory.ValidateAccessToken())
                return default(T);

            Before<T>(request);

            var client = _factory.GetRestClient();
            var response = await client.ExecuteAwaitable<T>(request);
            var data = response.Data;

            After(request, data);

            return data;
        }

        private void Before<T>(IRestRequest request)
        {
            foreach (var handler in _handlers)
                handler.BeforeRequest<T>(request);
        }

        private void After<T>(IRestRequest request, T data)
        {
            foreach (var handler in _handlers)
                handler.AfterRequest(request, data);
        }
    }
}