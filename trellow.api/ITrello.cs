using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using trellow.api.OAuth;

namespace TrelloNet
{
	public interface ITrello
	{
        IAsyncTrello Async { get; }

#if !WINDOWS_PHONE
		IMembers Members { get; }
		IBoards Boards { get; }
		ILists Lists { get; }
		ICards Cards { get; }
		IChecklists Checklists { get; }
		IOrganizations Organizations { get; }
		INotifications Notifications { get; }
		ITokens Tokens { get; }
		IActions Actions { get; }
		SearchResults Search(string query, int limit = 10, SearchFilter filter = null, IEnumerable<ModelType> modelTypes = null, DateTime? actionsSince = null, bool partial = false);
#endif
	    Task<OAuthToken> Verify(string verifier);

	    void Authorize(OAuthToken accessToken);
		
        void Deauthorize();

	    Task<Uri> GetAuthorizationUri(string applicationName, Scope scope, Expiration expiration, Uri callbackUri = null);
	}
}