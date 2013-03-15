using trellow.api.Boards;
using trellow.api.Boards.Internal;

namespace trellow.api.Checklists.Internal
{
	internal class ChecklistsForBoardRequest : BoardsRequest
	{
		public ChecklistsForBoardRequest(IBoardId board)
			: base(board, "checklists")
		{
		}
	}
}