using System;
using System.Threading.Tasks;
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

        public Func<Task<T>> Execution { get; set; }

        public May<T> Data { get; set; }

        public bool Handled { get; set; }

        public ResponseContext()
        {
            Data = new May<T>();
        }
    }

    public class RequestContext<T>
    {
        public string Resource { get; set; }

        public Method Method { get; set; }

        public Func<Task<T>> Execute { get; set; }

        public May<T> Data { get; set; }

        public RequestContext()
        {
            Data = new May<T>();
        } 
    }
}