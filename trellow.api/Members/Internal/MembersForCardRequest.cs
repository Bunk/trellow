using trellow.api.Cards;
using trellow.api.Cards.Internal;
using trellow.api.Internal;

namespace trellow.api.Members.Internal
{
	internal class MembersForCardRequest : CardsRequest
	{
		public MembersForCardRequest(ICardId card)
			: base(card, "members")
		{
			this.AddRequiredMemberFields();
		}
	}
}