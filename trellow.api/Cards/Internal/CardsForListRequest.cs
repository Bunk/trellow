using trellow.api.Internal;
using trellow.api.Lists;
using trellow.api.Lists.Internal;

namespace trellow.api.Cards.Internal
{
	internal class CardsForListRequest : ListsRequest
	{
		public CardsForListRequest(IListId list, CardFilter filter)
			: base(list, "cards")
		{
			this.AddCommonCardParameters();
			this.AddFilter(filter);
		}
	}
}