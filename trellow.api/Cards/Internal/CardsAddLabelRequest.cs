using RestSharp;
using trellow.api.Internal;

namespace trellow.api.Cards.Internal
{
	internal class CardsAddLabelRequest : CardsRequest
	{
		public CardsAddLabelRequest(ICardId card, Color color)
			: base(card, "labels", Method.POST)
		{
			this.AddValue(color.ToTrelloString());
		}
	}
}