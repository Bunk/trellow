using trellow.api.Lists;
using trellow.api.Lists.Internal;

namespace trellow.api.Boards.Internal
{
	internal class BoardsForListRequest : ListsRequest
	{
		public BoardsForListRequest(IListId list)
			: base(list, "board")
		{
		}
	}
}