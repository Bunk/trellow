using trellow.api.Boards;
using trellow.api.Boards.Internal;

namespace trellow.api.Members.Internal
{
	internal class MembersInvitedForBoardRequest : BoardsRequest
	{
		public MembersInvitedForBoardRequest(IBoardId board)
			: base(board, "membersInvited")
		{
			AddParameter("fields", "all");
		}
	}
}