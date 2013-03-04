using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrelloNet.Internal;
using trellow.api.Data;
using trellow.api.OAuth;

namespace TrelloNet
{
	public class Trello : ITrello
	{
	    private readonly INetworkService _networkService;
		private readonly IRequestClient _client;
	    private readonly IOAuth _auth;

	    public Trello(INetworkService networkService, IRequestClient client, IOAuth auth)
		{
		    _networkService = networkService;
		    _client = client;
		    _auth = auth;

		    Members = new AsyncMembers(_client);
            Boards = new AsyncBoards(_client);
            Lists = new AsyncLists(_client);
            Cards = new AsyncCards(_client);
            Checklists = new AsyncChecklists(_client);
            Organizations = new AsyncOrganizations(_client);
            Notifications = new AsyncNotifications(_client);
            Tokens = new AsyncTokens(_client);
            Actions = new AsyncActions(_client);
		}

        public IAsyncMembers Members { get; private set; }
        public IAsyncBoards Boards { get; private set; }
        public IAsyncLists Lists { get; private set; }
        public IAsyncCards Cards { get; private set; }
        public IAsyncChecklists Checklists { get; private set; }
        public IAsyncOrganizations Organizations { get; private set; }
        public IAsyncNotifications Notifications { get; private set; }
        public IAsyncTokens Tokens { get; private set; }
        public IAsyncActions Actions { get; private set; }

        public Task<SearchResults> Search(string query, int limit = 10, SearchFilter filter = null, IEnumerable<ModelType> modelTypes = null, DateTime? since = null, bool partial = false)
        {
            return _client.RequestAsync<SearchResults>(new SearchRequest(query, limit, filter, modelTypes, since, partial));
        }

	    public async Task<bool> AccessTokenIsFresh(OAuthToken accessToken)
	    {
	        try
	        {
                if (!_networkService.IsAvailable || accessToken == null)
                    return false;

	            var token = await Tokens.WithToken(accessToken.Key);
	            return token != null;
	        }
            catch (TrelloUnauthorizedException)
            {
                return false;
            }
            catch (TrelloException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
	    }

	    public Task<Uri> GetAuthorizationUri(string applicationName, Scope scope, Expiration expiration, Uri callbackUri = null)
	    {
	        return _auth.GetAuthorizationUri(applicationName, scope, expiration, callbackUri);
	    }

	    public Task<OAuthToken> Verify(string verifier)
	    {
	        return _auth.Verify(verifier);
	    }

	    public void Authorize(OAuthToken accessToken)
	    {
	        _auth.Authorize(accessToken);
	    }

	    public void Deauthorize()
	    {
	        _auth.Deauthorize();
	    }
	}
}