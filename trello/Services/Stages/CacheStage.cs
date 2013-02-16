using System.Threading.Tasks;
using JetBrains.Annotations;
using RestSharp;
using trellow.api.Data;
using trellow.api.Data.Stages;

namespace trello.Services.Stages
{
    [UsedImplicitly]
    public class CacheStage : RequestPipelineStage
    {
        private readonly ICache _cache;
        private readonly INetworkService _network;

        public CacheStage(ICache cache, INetworkService network)
        {
            _cache = cache;
            _network = network;
        }

        public override async Task<ResponseContext<T>> Handle<T>(ResponseContext<T> context)
        {
            var key = context.Client.BuildUri(context.Request).PathAndQuery;

            // Only GET requests are cached
            if (context.Request.Method == Method.GET)
            {
                if (_network.IsAvailable)
                {
                    // When connected, always prefer to pull fresh data
                    context = await ContinueIfPossible(context);

                    // Update the cache as well
                    _cache.Set(key, context.Data);
                }
                else
                {
                    // When disconnected, only pull from the cache when available
                    context.Data = _cache.Get<T>(key);

                    // Then continue through the pipeline with the cached data set
                    context = await ContinueIfPossible(context);
                }
            }
            else
            {
                context = await ContinueIfPossible(context);
            }

            return context;
        }

        public override async Task<RequestContext<T>> Handle<T>(RequestContext<T> context)
        {
            if (context.Method == Method.GET)
            {
                if (_network.IsAvailable)
                {
                    context = await ContinueIfPossible(context);
                    //_cache.Set(context.Resource, context.Data);
                }
                else
                {
                    context.Data = _cache.Get<T>(context.Resource);
                    context = await ContinueIfPossible(context);
                }
            }
            else
            {
                context = await ContinueIfPossible(context);
            }

            return context;
        }
    }
}