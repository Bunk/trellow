using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using trellow.api;
using trellow.api.Data;

namespace trello.Services.Cache
{
    public class CachingRestClient : IRequestClient
    {
        private readonly IRequestClient _requestClient;
        private readonly IRestClient _restClient;
        private readonly INetworkService _network;
        private readonly ICache _cache;

        public CachingRestClient(IRequestClient requestClient, IRestClient restClient, INetworkService network, ICache cache)
        {
            _requestClient = requestClient;
            _restClient = restClient;
            _network = network;
            _cache = cache;
        }

        public Task<IRestResponse> RequestAsync(IRestRequest request)
        {
            // No need to cache non-response requests
            return _requestClient.RequestAsync(request);
        }

        public async Task<T> RequestAsync<T>(IRestRequest request) where T : class, new()
        {
            var key = BuildKey(_restClient, request);

            if (request.Method == Method.GET)
            {
                // Prefer fresh data
                if (_network.IsAvailable)
                    return await RequestAndCache<T>(request, key);
                
                return Cached<T>(key);
            }
            
            return await _requestClient.RequestAsync<T>(request);
        }

        private static string BuildKey(IRestClient restClient, IRestRequest request)
        {
            return restClient.BuildUri(request).PathAndQuery;
        }

        private async Task<T> RequestAndCache<T>(IRestRequest request, string key) where T : class, new()
        {
            var data = await _requestClient.RequestAsync<T>(request);
            if (data != null)
            {
                _cache.Set(key, new Strilanc.Value.May<T>(data));
                {
                    return data;
                }
            }
            return null;
        }

        private async Task<IEnumerable<T>> RequestAndCacheList<T>(IRestRequest request, string key)
        {
            var data = await _requestClient.RequestListAsync<T>(request);
            if (data != null)
            {
                _cache.Set(key, new Strilanc.Value.May<IEnumerable<T>>(data));
                {
                    return data;
                }
            }
            return null;
        }

        private T Cached<T>(string key)
        {
            var data = _cache.Get<T>(key);
            if (data.HasValue)
                return (T)data;
            return default(T);
        } 

        public async Task<IEnumerable<T>> RequestListAsync<T>(IRestRequest request)
        {
            var key = BuildKey(_restClient, request);

            if (request.Method == Method.GET)
            {
                // Prefer fresh data
                if (_network.IsAvailable)
                    return await RequestAndCacheList<T>(request, key);

                return Cached<IEnumerable<T>>(key);
            }

            return await _requestClient.RequestListAsync<T>(request);
        }

        public Task<Uri> GetAuthorizationUri(string applicationName, Scope scope, Expiration expiration, Uri callbackUri = null)
        {
            return _requestClient.GetAuthorizationUri(applicationName, scope, expiration, callbackUri);
        }

        public Task<OAuthToken> Verify(string verifier)
        {
            return _requestClient.Verify(verifier);
        }

        public void Authorize(OAuthToken accessToken)
        {
            _requestClient.Authorize(accessToken);
        }

        public void Deauthorize()
        {
            _requestClient.Deauthorize();
        }
    }
}