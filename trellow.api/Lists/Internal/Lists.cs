using System.Collections.Generic;

#if !WINDOWS_PHONE
namespace TrelloNet.Internal
{
	internal class Lists : ILists
	{
		private readonly TrelloRestClient _restClient;

		public Lists(TrelloRestClient restClient)
		{
			_restClient = restClient;
		}

		public List WithId(string listId)
		{
			return _restClient.Request<List>(new ListsRequest(listId));
		}

		public List ForCard(ICardId card)
		{
			return _restClient.Request<List>(new ListsForCardRequest(card));
		}

		public IEnumerable<List> ForBoard(IBoardId board, ListFilter filter = ListFilter.Open)
		{
			return _restClient.Request<List<List>>(new ListsForBoardRequest(board, filter));
		}

		public List Add(NewList list)
		{
			return _restClient.Request<List>(new ListsAddRequest(list));
		}

		public List Add(string name, IBoardId board)
		{
			return Add(new NewList(name, board));
		}

		public void Archive(IListId list)
		{
			_restClient.Request(new ListsArchiveRequest(list));
		}

		public void Unarchive(IListId list)
		{
			_restClient.Request(new ListsUnarchiveRequest(list));
		}

		public void ChangeName(IListId list, string name)
		{
			_restClient.Request(new ListsChangeNameRequest(list, name));
		}

		public void Update(IUpdatableList list)
		{
			_restClient.Request(new ListsUpdateRequest(list));
		}

		public void ChangePos(IListId list, double pos)
		{
			_restClient.Request(new ListsChangePosRequest(list, pos));
		}

		public void ChangePos(IListId list, Position pos)
		{
			_restClient.Request(new ListsChangePosRequest(list, pos));
		}
	}
}
#endif