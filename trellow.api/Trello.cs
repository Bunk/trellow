using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrelloNet.Internal;
using trellow.api.OAuth;

namespace TrelloNet
{
	public class Trello : ITrello
	{
	    private readonly OAuthToken _apiKey;
		private TrelloRestClient _restClient;

		public Trello(string publicKey, string privateKey)
		{
		    _apiKey = new OAuthToken(publicKey, privateKey);
			_restClient = new TrelloRestClient(_apiKey);

            Async = new AsyncTrello(_restClient);
		}

        public IAsyncTrello Async { get; private set; }
#if !WINDOWS_PHONE
		public SearchResults Search(string query, int limit = 10, SearchFilter filter = null, IEnumerable<ModelType> modelTypes = null, DateTime? actionsSince = null, bool partial = false)
		{
			return _restClient.Request<SearchResults>(new SearchRequest(query, limit, filter, modelTypes, actionsSince, partial));
		}
#endif

        public Task<OAuthToken> Verify(string verifier)
        {
            return _restClient.AuthorizeRequest(verifier);
        }

		public void Authorize(OAuthToken accessToken)
		{
		    _restClient.AuthorizeAccess(accessToken);
		}

		public void Deauthorize()
		{
		    _restClient = new TrelloRestClient(_apiKey);
		}

        public Task<Uri> GetAuthorizationUri(string applicationName, Scope scope, Expiration expiration, Uri callbackUri = null)
        {
            var task = _restClient.GetAuthorizationUri(applicationName, scope, expiration, callbackUri);
            return task;
        }
	}
}