using trellow.api.Internal;
using trellow.api.Members;
using trellow.api.Members.Internal;

namespace trellow.api.Boards.Internal
{
	internal class BoardsForMemberRequest : MembersRequest
	{
		public BoardsForMemberRequest(IMemberId member, BoardFilter filter)
			: base(member, "boards")
		{
			this.AddFilter(filter);
		}
	}
}