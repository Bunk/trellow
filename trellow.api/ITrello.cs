using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using trellow.api.OAuth;

namespace TrelloNet
{
	public interface ITrello : IOAuth
	{
        IAsyncMembers Members { get; }
        IAsyncBoards Boards { get; }
        IAsyncLists Lists { get; }
        IAsyncCards Cards { get; }
        IAsyncChecklists Checklists { get; }
        IAsyncOrganizations Organizations { get; }
        IAsyncNotifications Notifications { get; }
        IAsyncTokens Tokens { get; }
        IAsyncActions Actions { get; }
        Task<SearchResults> Search(string query, int limit = 10, SearchFilter filter = null, IEnumerable<ModelType> modelTypes = null, DateTime? since = null, bool partial = false);
        Task<bool> AccessTokenIsFresh(OAuthToken accessToken);
	}
}