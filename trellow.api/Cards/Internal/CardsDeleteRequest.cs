using RestSharp;

namespace trellow.api.Cards.Internal
{
	internal class CardsDeleteRequest : CardsRequest
	{
		public CardsDeleteRequest(ICardId card)
			: base(card, method: Method.DELETE)
		{			
		}
	}
}