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

namespace trellow.api.Internal
{
	internal class AsyncTrello : IAsyncTrello
	{
		private readonly TrelloRestClient _restClient;

		internal AsyncTrello(TrelloRestClient restClient)
		{
			_restClient = restClient;

			Members = new AsyncMembers(_restClient);
			Boards = new AsyncBoards(_restClient);
			Lists = new AsyncLists(_restClient);
			Cards = new AsyncCards(_restClient);
			Checklists = new AsyncChecklists(_restClient);
			Organizations = new AsyncOrganizations(_restClient);
			Notifications = new AsyncNotifications(_restClient);
			Tokens = new AsyncTokens(_restClient);
			Actions = new AsyncActions(_restClient);
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
			return _restClient.RequestAsync<SearchResults>(new SearchRequest(query, limit, filter, modelTypes, since, partial));
		}
	}
}