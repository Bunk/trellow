using System;
using RestSharp;
using Strilanc.Value;

namespace trellow.api.Data.Stages
{
    public class ResponseContext<T>
    {
        public IRestClient Client { get; set; }

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
}