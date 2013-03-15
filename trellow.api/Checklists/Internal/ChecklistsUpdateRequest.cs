using RestSharp;
using trellow.api.Internal;

namespace trellow.api.Checklists.Internal
{
	internal class ChecklistsUpdateRequest : ChecklistsRequest
	{
		public ChecklistsUpdateRequest(IUpdatableChecklist checklist)
			: base(checklist.Id, method: Method.PUT)
		{
			Guard.RequiredTrelloString(checklist.Name, "name");

			AddParameter("name", checklist.Name);
		}
	}
}