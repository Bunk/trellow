using RestSharp;
using trellow.api.Internal;

namespace trellow.api.Cards.Internal
{
	internal class CardsChangeDescriptionRequest : CardsRequest
	{
		public CardsChangeDescriptionRequest(ICardId card, string description)
			: base(card, "desc", Method.PUT)
		{
			Guard.OptionalTrelloString(description, "desc");

			this.AddValue(description);
		}
	}
}