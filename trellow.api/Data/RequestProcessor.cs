using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using Strilanc.Value;
using trellow.api.OAuth;

namespace trellow.api.Data
{
    public interface IRequestProcessor
    {
        Task<May<T>> Execute<T>(IRestRequest request);
    }

    public interface IHandleRequests
    {
        RequestContext<T>  BeforeRequest<T>(RequestContext<T> context);

        ResponseContext<T>  AfterRequest<T>(ResponseContext<T> context);
    }

    public class RequestContext<T>
    {
        public Uri RequestUri { get; set; }

        public IRestRequest Request { get; set; }

        public Exception Exception { get; set; }

        public May<T> Data { get; set; }

        public bool Handled { get; set; }

        public RequestContext()
        {
            Data = new May<T>();
        } 
    }

    public class ResponseContext<T>
    {
        public Uri RequestUri { get; set; }

        public IRestRequest Request { get; set; }

        public IRestResponse<T> Response { get; set; }

        public Exception Exception { get; set; }

        public May<T> Data { get; set; }

        public bool Handled { get; set; }

        public ResponseContext()
        {
            Data = new May<T>();
        } 
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

        public async Task<May<T>> Execute<T>(IRestRequest request)
        {
            if (!_factory.ValidateAccessToken())
                return default(T);

            var client = _factory.GetRestClient();
            var uri = client.BuildUri(request);

            var before = Before(new RequestContext<T> { Request = request, RequestUri = uri });
            var response = new ResponseContext<T> { Request = request, RequestUri = uri };
            if (before.Data.HasValue)
            {
                response.Data = before.Data.ForceGetValue();
            }
            else
            {
                var r = await client.ExecuteAwaitable<T>(request);
                response.Response = r;
                response.Data = r.Data;
            }

            var after = After(response);
            return after.Data;
        }

        private RequestContext<T> Before<T>(RequestContext<T> context)
        {
            var current = context;
            foreach (var handler in _handlers)
            {
                if (!current.Handled)
                {
                    current = handler.BeforeRequest(current);
                }
            }
            return current;
        }

        private ResponseContext<T> After<T>(ResponseContext<T> context)
        {
            var current = context;
            foreach (var handler in _handlers)
            {
                if (!current.Handled)
                {
                    current = handler.AfterRequest(current);
                }
            }
            return current;
        }
    }
}