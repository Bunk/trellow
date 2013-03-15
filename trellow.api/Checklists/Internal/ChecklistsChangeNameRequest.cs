using RestSharp;
using trellow.api.Internal;

namespace trellow.api.Checklists.Internal
{
	internal class ChecklistsChangeNameRequest : ChecklistsRequest
	{
		public ChecklistsChangeNameRequest(IChecklistId checklist, string name)
			: base(checklist, "name", Method.PUT)
		{
			Guard.RequiredTrelloString(name, "name");
			this.AddValue(name);
		}
	}
}