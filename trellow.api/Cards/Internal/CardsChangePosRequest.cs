using RestSharp;
using trellow.api.Internal;

namespace trellow.api.Cards.Internal
{
	internal class CardsChangePosRequest : CardsRequest
	{
		public CardsChangePosRequest(ICardId card, double pos)
			: base(card, "pos", Method.PUT)
		{
			Guard.Positive(pos, "pos");
			this.AddValue(pos);
		}

		public CardsChangePosRequest(ICardId card, Position pos)
			: base(card, "pos", Method.PUT)
		{
			this.AddValue(pos.ToTrelloString());
		}
	}
}