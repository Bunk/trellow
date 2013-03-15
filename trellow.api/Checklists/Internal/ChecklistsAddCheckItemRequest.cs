using RestSharp;
using trellow.api.Internal;

namespace trellow.api.Checklists.Internal
{
	internal class ChecklistsAddCheckItemRequest : ChecklistsRequest
	{
		public ChecklistsAddCheckItemRequest(IChecklistId checklist, string name) 
			: base(checklist, "checkitems", Method.POST)
		{
			Guard.RequiredTrelloString(name, "name");
			AddParameter("name", name);
		}
	}
}