using RestSharp;
using trellow.api.Internal;

namespace trellow.api.Checklists.Internal
{
	internal class ChecklistsRemoveCheckItemRequest : ChecklistsRequest
	{
		public ChecklistsRemoveCheckItemRequest(IChecklistId checklist, string checkItemId)
			: base(checklist, "checkitems/{idCheckItem}", Method.DELETE)
		{
			Guard.NotNullOrEmpty(checkItemId, "checkItemId");
			AddParameter("idCheckItem", checkItemId, ParameterType.UrlSegment);
		}
	}
}