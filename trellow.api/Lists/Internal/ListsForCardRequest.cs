using trellow.api.Cards;
using trellow.api.Cards.Internal;

namespace trellow.api.Lists.Internal
{
	internal class ListsForCardRequest : CardsRequest
	{
		public ListsForCardRequest(ICardId card)
			: base(card, "list")
		{
		}
	}
}