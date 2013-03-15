using System.Collections.Generic;
using trellow.api.Internal;
using trellow.api.Lists;
using trellow.api.Lists.Internal;

namespace trellow.api.Actions.Internal
{
	internal class ActionsForListRequest : ListsRequest
	{
		public ActionsForListRequest(IListId list, ISince since, Paging paging, IEnumerable<ActionType> filter)
			: base(list, "actions")
		{
			this.AddTypeFilter(filter);
			this.AddSince(since);
			this.AddPaging(paging);
		}
	}
}