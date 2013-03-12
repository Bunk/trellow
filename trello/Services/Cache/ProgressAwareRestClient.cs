using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestSharp;
using TrelloNet;
using TrelloNet.Internal;
using trello.Services.Handlers;
using trellow.api.OAuth;

namespace trello.Services.Cache
{
    public class ProgressAwareRestClient : IRequestClient
    {
        private readonly IRequestClient _client;
        private readonly IProgressService _progress;

        public ProgressAwareRestClient(IRequestClient client, IProgressService progress)
        {
            _client = client;
            _progress = progress;
        }

        public async Task<IRestResponse> RequestAsync(IRestRequest request)
        {
            using (new ProgressScope(_progress, GetMessageFor(request)))
            {
                return await _client.RequestAsync(request);
            }
        }

        public async Task<T> RequestAsync<T>(IRestRequest request) where T : class, new()
        {
            using (new ProgressScope(_progress, GetMessageFor(request)))
            {
                return await _client.RequestAsync<T>(request);
            }
        }

        public async Task<IEnumerable<T>> RequestListAsync<T>(IRestRequest request)
        {
            using (new ProgressScope(_progress, GetMessageFor(request)))
                return await _client.RequestListAsync<T>(request);
        }

        public Task<Uri> GetAuthorizationUri(string applicationName, Scope scope, Expiration expiration, Uri callbackUri = null)
        {
            return _client.GetAuthorizationUri(applicationName, scope, expiration, callbackUri);
        }

        public Task<OAuthToken> Verify(string verifier)
        {
            return _client.Verify(verifier);
        }

        public void Authorize(OAuthToken accessToken)
        {
            _client.Authorize(accessToken);
        }

        public void Deauthorize()
        {
            _client.Deauthorize();
        }

        private string GetMessageFor(IRestRequest request)
        {
            if (request.Method == Method.POST)
                return "updating...";
            if (request.Method == Method.PUT)
                return "adding...";
            if (request.Method == Method.DELETE)
                return "deleting...";

            return "loading...";
        }
    }
}