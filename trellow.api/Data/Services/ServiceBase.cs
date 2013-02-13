using RestSharp;

namespace trellow.api.Data.Services
{
    public abstract class ServiceBase
    {
        protected readonly IRequestProcessor Processor;

        protected ServiceBase(IRequestProcessor processor)
        {
            Processor = processor;
        }

        protected static IRestRequest Request(string resource)
        {
            var request = new RestRequest(resource)
            {
                RequestFormat = DataFormat.Json
            };

            return request;
        }

        protected static IRestRequest Update(string resource)
        {
            var request = new RestRequest(resource, Method.PUT)
                {
                    RequestFormat = DataFormat.Json
                };

            return request;
        }

        protected static IRestRequest Post(string resource)
        {
            var request = new RestRequest(resource, Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            return request;
        }

        protected static IRestRequest Delete(string resource)
        {
            var request = new RestRequest(resource, Method.DELETE)
            {
                RequestFormat = DataFormat.Json
            };
            return request;
        }
    }
}