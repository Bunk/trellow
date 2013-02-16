using System.Threading.Tasks;
using TrelloNet;
using trellow.api.Data.Services;

namespace trellow.api.Data.Stages
{
    public class CallExternalApiStage : RequestPipelineStage
    {
        private readonly INetworkService _networkService;

        public CallExternalApiStage(INetworkService networkService)
        {
            _networkService = networkService;
        }

        public override async Task<ResponseContext<T>> Handle<T>(ResponseContext<T> context)
        {
            if (_networkService.IsAvailable && !context.Data.HasValue)
            {
                var response = await context.Client.ExecuteAwaitable<T>(context.Request);
                context.Response = response;
                context.Data = response.Data;
            }

            return await ContinueIfPossible(context);
        }

        public override async Task<RequestContext<T>> Handle<T>(RequestContext<T> context)
        {
            if (_networkService.IsAvailable && !context.Data.HasValue)
            {
                context.Data = await context.Execute();
            }

            return await ContinueIfPossible(context);
        }
    }
}