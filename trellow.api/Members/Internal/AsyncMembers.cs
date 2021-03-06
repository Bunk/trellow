using System.Collections.Generic;
using System.Threading.Tasks;
using trellow.api.Boards;
using trellow.api.Cards;
using trellow.api.Organizations;
using trellow.api.Search;
using trellow.api.Search.Internal;

namespace trellow.api.Members.Internal
{
	public class AsyncMembers : IAsyncMembers
	{
		private readonly IRequestClient _restClient;

	    public AsyncMembers(IRequestClient restClient)
		{
		    _restClient = restClient;
		}

	    public Task<Member> WithId(string memberIdOrUsername)
		{
			return _restClient.RequestAsync<Member>(new MembersRequest(memberIdOrUsername));
		}

		public Task<Member> Me()
		{
			return _restClient.RequestAsync<Member>(new MembersRequest(new Me()));
		}

		public Task<IEnumerable<Member>> ForBoard(IBoardId board, MemberFilter filter = MemberFilter.All)
		{
			return _restClient.RequestListAsync<Member>(new MembersForBoardRequest(board, filter));
		}

		public Task<IEnumerable<Member>> ForCard(ICardId card)
		{
			return _restClient.RequestListAsync<Member>(new MembersForCardRequest(card));
		}

		public Task<IEnumerable<Member>> ForOrganization(IOrganizationId organization, MemberFilter filter = MemberFilter.All)
		{
			return _restClient.RequestListAsync<Member>(new MembersForOrganizationRequest(organization, filter));
		}

		public Task<Member> ForToken(string token)
		{
			return _restClient.RequestAsync<Member>(new MembersForTokenRequest(token));
		}

		public Task<IEnumerable<Member>> InvitedForBoard(IBoardId board)
		{
			return _restClient.RequestListAsync<Member>(new MembersInvitedForBoardRequest(board));
		}

		public Task<IEnumerable<Member>> Search(string query, int limit = 10, SearchFilter filter = null, bool partial = false)
		{
			return _restClient.RequestAsync<SearchResults>(new SearchRequest(query, limit, filter, new[] { ModelType.Members }, null, partial))
				.ContinueWith<IEnumerable<Member>>(r => r.Result.Members);
		}
	}
}