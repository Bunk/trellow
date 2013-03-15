using trellow.api.Boards;
using trellow.api.Boards.Internal;

namespace trellow.api.Organizations.Internal
{
	internal class OrganizationsForBoardRequest : BoardsRequest
	{
		public OrganizationsForBoardRequest(IBoardId board)
			: base(board, "organization")
		{
		}
	}
}