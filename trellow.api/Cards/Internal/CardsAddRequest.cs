using RestSharp;

namespace TrelloNet.Internal
{
	internal class CardsAddRequest : RestRequest
	{
		public CardsAddRequest(NewCard card)
			: base("cards", Method.POST)
		{
			Guard.NotNull(card, "card");
			Guard.NotNull(card.IdList, "card.IdList");
			Guard.RequiredTrelloString(card.Name, "card.Name");

			AddParameter("name", card.Name);
			AddParameter("idList", card.IdList.GetListId());

			if (!string.IsNullOrEmpty(card.Desc))
			{
				Guard.OptionalTrelloString(card.Desc, "desc");
				AddParameter("desc", card.Desc);
			}
		}
	}
}