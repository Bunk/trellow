using trellow.api.Cards;
using trellow.api.Cards.Internal;

namespace trellow.api.Checklists.Internal
{
	internal class ChecklistsForCardRequest : CardsRequest
	{
		public ChecklistsForCardRequest(ICardId card)
			: base(card, "checklists")
		{
		}
	}
}