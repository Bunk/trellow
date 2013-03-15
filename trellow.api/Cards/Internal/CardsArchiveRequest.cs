using RestSharp;
using trellow.api.Internal;

namespace trellow.api.Cards.Internal
{
	internal class CardsArchiveRequest : CardsRequest
	{
		public CardsArchiveRequest(ICardId card)
			: base(card, "closed", Method.PUT)
		{
			this.AddValue("true");			
		}
	}
}