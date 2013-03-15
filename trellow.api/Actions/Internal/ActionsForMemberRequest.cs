using System.Collections.Generic;
using trellow.api.Internal;
using trellow.api.Members;
using trellow.api.Members.Internal;

namespace trellow.api.Actions.Internal
{
	internal class ActionsForMemberRequest : MembersRequest
	{
		public ActionsForMemberRequest(IMemberId member, ISince since, Paging paging, IEnumerable<ActionType> filter)
			: base(member, "actions")
		{
			this.AddTypeFilter(filter);
			this.AddSince(since);
			this.AddPaging(paging);
		}
	}
}