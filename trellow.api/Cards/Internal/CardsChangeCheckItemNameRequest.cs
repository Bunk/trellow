using trellow.api.Checklists;
using trellow.api.Internal;

namespace trellow.api.Cards.Internal
{
	internal class CardsChangeCheckItemNameRequest : CardChangeCheckItemRequest
	{
		public CardsChangeCheckItemNameRequest(ICardId card, IChecklistId checkList, ICheckItemId checkItem, string name)
			: base(card, checkList, checkItem, "name")
		{
			Guard.RequiredTrelloString(name, "name");
			this.AddValue(name);
		}
	}
}