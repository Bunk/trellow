using System.Threading.Tasks;
using RestSharp;
using Strilanc.Value;
using trellow.api.Data.Stages;
using trellow.api.OAuth;

namespace trellow.api.Data
{
    public interface IRequestProcessor
    {
        Task<May<T>> Execute<T>(IRestRequest request);
    }

    public class RequestProcessor : IRequestProcessor
    {
        private readonly IOAuthClient _factory;
        private readonly IRequestPipeline _pipelineFactory;

        public RequestProcessor(IOAuthClient factory, IRequestPipeline pipelineFactory)
        {
            _factory = factory;
            _pipelineFactory = pipelineFactory;
        }

        public async Task<May<T>> Execute<T>(IRestRequest request)
        {
            if (!_factory.ValidateAccessToken())
                return May<T>.NoValue;

            var client = _factory.GetRestClient();
            var context = new ResponseContext<T>
            {
                Client = client,
                Request = request
            };

            var pipeline = _pipelineFactory.Build();
            context = await pipeline.Handle(context);
            return context.Data;
        }
    }
}