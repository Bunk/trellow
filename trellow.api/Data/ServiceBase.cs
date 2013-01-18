using System;
using RestSharp;

namespace trellow.api.Data
{
    public abstract class ServiceBase
    {
        protected readonly IRequestProcessor Processor;

        protected ServiceBase(IRequestProcessor processor)
        {
            Processor = processor;
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
    }
}