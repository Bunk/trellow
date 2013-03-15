using RestSharp;
using trellow.api.Internal;

namespace trellow.api.Cards.Internal
{
	internal class CardsUnarchiveRequest : CardsRequest
	{
		public CardsUnarchiveRequest(ICardId card)
			: base(card, "closed", Method.PUT)
		{
			this.AddValue("false");			
		}
	}
}