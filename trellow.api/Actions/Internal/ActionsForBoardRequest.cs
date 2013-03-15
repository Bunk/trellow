using System.Collections.Generic;
using trellow.api.Boards;
using trellow.api.Boards.Internal;
using trellow.api.Internal;

namespace trellow.api.Actions.Internal
{
	internal class ActionsForBoardRequest : BoardsRequest
	{
		public ActionsForBoardRequest(IBoardId board, ISince since, Paging paging, IEnumerable<ActionType> types)
			: base(board, "actions")
		{
			this.AddTypeFilter(types);
			this.AddSince(since);
			this.AddPaging(paging);
		}		
	}
}