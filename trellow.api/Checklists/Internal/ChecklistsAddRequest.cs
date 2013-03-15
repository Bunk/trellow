using RestSharp;
using trellow.api.Boards;
using trellow.api.Boards.Internal;
using trellow.api.Internal;

namespace trellow.api.Checklists.Internal
{
	internal class ChecklistsAddRequest : BoardsRequest
	{
		public ChecklistsAddRequest(IBoardId board, string name)
			: base(board, "checklists", Method.POST)
		{
			Guard.RequiredTrelloString(name, "name");
			AddParameter("name", name);
		}
	}
}