using RestSharp;
using trellow.api.Checklists;
using trellow.api.Internal;

namespace trellow.api.Cards.Internal
{
	internal abstract class CardChangeCheckItemRequest : CardsRequest
	{
		protected CardChangeCheckItemRequest(ICardId card, IChecklistId checkList, ICheckItemId checkItem, string resource)
			: base(card, "checklist/{idCheckList}/checkItem/{idCheckItem}/" + resource, Method.PUT)
		{
			Guard.NotNull(checkList, "checkList");
			Guard.NotNull(checkItem, "checkItem");

			AddParameter("idCheckList", checkList.GetChecklistId(), ParameterType.UrlSegment);
			AddParameter("idCheckItem", checkItem.GetCheckItemId(), ParameterType.UrlSegment);
		}
	}
}