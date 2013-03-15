using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using trellow.api.Actions;
using trellow.api.Actions.Internal;
using trellow.api.Boards;
using trellow.api.Boards.Internal;
using trellow.api.Cards;
using trellow.api.Cards.Internal;
using trellow.api.Checklists;
using trellow.api.Checklists.Internal;
using trellow.api.Data;
using trellow.api.Lists;
using trellow.api.Lists.Internal;
using trellow.api.Members;
using trellow.api.Members.Internal;
using trellow.api.Notifications;
using trellow.api.Notifications.Internal;
using trellow.api.Organizations;
using trellow.api.Organizations.Internal;
using trellow.api.Search;
using trellow.api.Search.Internal;
using trellow.api.Tokens;
using trellow.api.Tokens.Internal;

namespace trellow.api
{
	public class Trello : ITrello
	{
	    private readonly INetworkService _networkService;
        private readonly IRequestClient _client;

	    public Trello(INetworkService networkService, IRequestClient client)
		{
		    _networkService = networkService;
		    _client = client;

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
	}
}