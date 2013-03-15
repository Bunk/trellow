using trellow.api.Cards;
using trellow.api.Cards.Internal;

namespace trellow.api.Boards.Internal
{
	internal class BoardsForCardRequest : CardsRequest
	{
		public BoardsForCardRequest(ICardId card)
			: base(card, "board")
		{
		}
	}
}