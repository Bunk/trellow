using RestSharp;
using trellow.api.Checklists;
using trellow.api.Internal;

namespace trellow.api.Cards.Internal
{
	internal class CardsRemoveChecklistRequest : CardsRequest
	{
		public CardsRemoveChecklistRequest(ICardId card, IChecklistId checklist)
			: base(card, "checklists/{idChecklist}", Method.DELETE)
		{
			Guard.NotNull(checklist, "checklist");
			AddParameter("idChecklist", checklist.GetChecklistId(), ParameterType.UrlSegment);
		}
	}
}