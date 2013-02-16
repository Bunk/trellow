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
#if !WINDOWS_PHONE
			Members = new Members(_restClient);
			Boards = new Boards(_restClient);
			Lists = new Lists(_restClient);
			Cards = new Cards(_restClient);
			Checklists = new Checklists(_restClient);
			Organizations = new Organizations(_restClient);
			Notifications = new Notifications(_restClient);
			Tokens = new Tokens(_restClient);
			Actions = new Actions(_restClient);
#endif
		}

        public IAsyncTrello Async { get; private set; }
#if !WINDOWS_PHONE
		public IMembers Members { get; private set; }
		public IBoards Boards { get; private set; }
		public ILists Lists { get; private set; }
		public ICards Cards { get; private set; }
		public IChecklists Checklists { get; private set; }
		public IOrganizations Organizations { get; private set; }
		public INotifications Notifications { get; private set; }
		public ITokens Tokens { get; private set; }
		public IActions Actions { get; private set; }

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