using trellow.api.Checklists;
using trellow.api.Checklists.Internal;

namespace trellow.api.Boards.Internal
{
	internal class BoardsForChecklistRequest : ChecklistsRequest
	{
		public BoardsForChecklistRequest(IChecklistId checkList)
			: base(checkList, "board")
		{
		}
	}
}