using System.Net.NetworkInformation;
using JetBrains.Annotations;
using RestSharp;
using trellow.api.Data;

namespace trello.Services
{
    [UsedImplicitly]
    public class CacheAwareRequestHandler : IHandleRequests
    {
        private readonly ICache _cache;

        public CacheAwareRequestHandler(ICache cache)
        {
            _cache = cache;
        }

        public RequestContext<T>  BeforeRequest<T>(RequestContext<T> context)
        {
            var key = context.RequestUri.PathAndQuery;

            // GET requests only
            if (context.Request.Method != Method.GET)
                return context;

            // Don't use the cache if it's not cached and we're connected
            if (!_cache.Contains(key) && NetworkInterface.GetIsNetworkAvailable())
                return context;

            // Don't use the cache if the item has expired and we're connected
            if (_cache.Expired(key) && NetworkInterface.GetIsNetworkAvailable())
                return context;

            context.Data = _cache.Get<T>(key);

            return context;
        }

        public ResponseContext<T> AfterRequest<T>(ResponseContext<T> context)
        {
            var key = context.RequestUri.PathAndQuery;

            if (context.Request.Method != Method.GET)
                return context;

            if (context.Data.HasValue)
                _cache.Set(key, context.Data);

            return context;
        }
    }
}