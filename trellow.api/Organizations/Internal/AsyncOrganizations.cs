using System.Collections.Generic;
using System.Threading.Tasks;
using trellow.api.Boards;
using trellow.api.Members;
using trellow.api.Search;
using trellow.api.Search.Internal;

namespace trellow.api.Organizations.Internal
{
	internal class AsyncOrganizations : IAsyncOrganizations
	{
		private readonly IRequestClient _restClient;

		public AsyncOrganizations(IRequestClient restClient)
		{
			_restClient = restClient;
		}

		public Task<Organization> WithId(string orgIdOrName)
		{
			return _restClient.RequestAsync<Organization>(new OrganizationsRequest(orgIdOrName));
		}

		public Task<Organization> ForBoard(IBoardId board)
		{
			return _restClient.RequestAsync<Organization>(new OrganizationsForBoardRequest(board));
		}

		public Task<IEnumerable<Organization>> ForMember(IMemberId member, OrganizationFilter filter = OrganizationFilter.All)
		{
			return _restClient.RequestListAsync<Organization>(new OrganizationsForMemberRequest(member, filter));
		}

		public Task<IEnumerable<Organization>> ForMe(OrganizationFilter filter = OrganizationFilter.All)
		{
			return ForMember(new Me(), filter);
		}

		public Task<IEnumerable<Organization>> Search(string query, int limit = 10, SearchFilter filter = null, bool partial = false)
		{
			return _restClient.RequestAsync<SearchResults>(new SearchRequest(query, limit, filter, new[] { ModelType.Organizations }, null, partial))
				.ContinueWith<IEnumerable<Organization>>(r => r.Result.Organizations);		
		}
	}
}