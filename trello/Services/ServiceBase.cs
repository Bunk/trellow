using System;
using System.Threading.Tasks;
using RestSharp;

namespace trello.Services
{
    public abstract class ServiceBase
    {
        private readonly IOAuthClient _factory;

        protected ServiceBase(IOAuthClient factory)
        {
            _factory = factory;
        }

        protected IRestRequest Request(string resource)
        {
            var request = new RestRequest(resource)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddParameter("r", DateTime.Now.Ticks); // no cache

            return request;
        }

        protected async Task<T> Execute<T>(IRestRequest request)
        {
            if (!_factory.ValidateAccessToken())
                return default(T);

            var client = _factory.GetRestClient();
            var response = await client.ExecuteAwaitable<T>(request);
            return response.Data;
        }
    }
}