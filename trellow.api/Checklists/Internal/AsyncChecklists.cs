using System.Collections.Generic;
using System.Threading.Tasks;
using trellow.api.Boards;
using trellow.api.Cards;

namespace trellow.api.Checklists.Internal
{
	internal class AsyncChecklists : IAsyncChecklists
	{
		private readonly IRequestClient _restClient;

		public AsyncChecklists(IRequestClient restClient)
		{
			_restClient = restClient;
		}

		public Task<Checklist> WithId(string checklistId)
		{
			return _restClient.RequestAsync<Checklist>(new ChecklistsRequest(checklistId));
		}

		public Task<IEnumerable<Checklist>> ForBoard(IBoardId board)
		{
			return _restClient.RequestListAsync<Checklist>(new ChecklistsForBoardRequest(board));
		}

		public Task<IEnumerable<Checklist>> ForCard(ICardId card)
		{
			return _restClient.RequestListAsync<Checklist>(new ChecklistsForCardRequest(card));
		}

		public Task<Checklist> Add(string name, IBoardId board)
		{
			return _restClient.RequestAsync<Checklist>(new ChecklistsAddRequest(board, name));
		}

		public Task ChangeName(IChecklistId checklist, string name)
		{
			return _restClient.RequestAsync(new ChecklistsChangeNameRequest(checklist, name));
		}

		public Task<CheckItem> AddCheckItem(IChecklistId checklist, string name)
		{
			return _restClient.RequestAsync<CheckItem>(new ChecklistsAddCheckItemRequest(checklist, name));
		}

		public Task RemoveCheckItem(IChecklistId checklist, string checkItemId)
		{
			return _restClient.RequestAsync(new ChecklistsRemoveCheckItemRequest(checklist, checkItemId));
		}

		public Task Update(IUpdatableChecklist checklist)
		{
			return _restClient.RequestAsync(new ChecklistsUpdateRequest(checklist));
		}
	}
}