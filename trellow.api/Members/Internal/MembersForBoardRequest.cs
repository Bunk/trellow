using trellow.api.Boards;
using trellow.api.Boards.Internal;
using trellow.api.Internal;

namespace trellow.api.Members.Internal
{
	internal class MembersForBoardRequest : BoardsRequest
	{
		public MembersForBoardRequest(IBoardId board, MemberFilter filter)
			: base(board, "members")
		{			
			this.AddFilter(filter);
			this.AddRequiredMemberFields();
		}
	}
}