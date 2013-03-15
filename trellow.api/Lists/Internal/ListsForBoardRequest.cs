using trellow.api.Boards;
using trellow.api.Boards.Internal;
using trellow.api.Internal;

namespace trellow.api.Lists.Internal
{
	internal class ListsForBoardRequest : BoardsRequest
	{
		public ListsForBoardRequest(IBoardId board, ListFilter filter)
			: base(board, "lists")
		{
			this.AddFilter(filter);
		}
	}
}