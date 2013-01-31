using System.Net.NetworkInformation;
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
            // Only pull from the cache when not connected to the network
            if (context.Request.Method == Method.GET && _network.IsAvailable)
            {
                context.Data = _cache.Get<T>(key);
                // REVIEW: Should we call ContinueIfPossible?
            }
            else
            {
                // Otherwise, defer to later stages for data retrieval
                context = await ContinueIfPossible(context);
                
                // And cache the results
                _cache.Set(key, context.Data);
            }

            return context;
        }
    }
}