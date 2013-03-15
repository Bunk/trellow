using RestSharp;
using trellow.api.Internal;
using trellow.api.Lists;

namespace trellow.api.Cards.Internal
{
	internal class CardsMoveRequest : CardsRequest
	{
		public CardsMoveRequest(ICardId card, IListId list)
			: base(card, "idList", Method.PUT)
		{
			Guard.NotNull(list, "list");
			this.AddValue(list.GetListId());
		}
	}
}