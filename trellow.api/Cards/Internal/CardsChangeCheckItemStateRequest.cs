using trellow.api.Checklists;
using trellow.api.Internal;

namespace trellow.api.Cards.Internal
{
	internal class CardsChangeCheckItemStateRequest : CardChangeCheckItemRequest
	{
		public CardsChangeCheckItemStateRequest(ICardId card, IChecklistId checkList, ICheckItemId checkItem, bool check)
			: base(card, checkList, checkItem, "state")
		{
			this.AddValue(check.ToTrelloString());
		}
	}
}