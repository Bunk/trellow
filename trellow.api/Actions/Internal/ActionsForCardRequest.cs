using System.Collections.Generic;
using trellow.api.Cards;
using trellow.api.Cards.Internal;
using trellow.api.Internal;

namespace trellow.api.Actions.Internal
{
	internal class ActionsForCardRequest : CardsRequest
	{
		public ActionsForCardRequest(ICardId card, ISince since, Paging paging, IEnumerable<ActionType> filter)
			: base(card, "actions")
		{
			this.AddTypeFilter(filter);
			this.AddSince(since);
			this.AddPaging(paging);
		}
	}
}