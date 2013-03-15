using trellow.api.Internal;
using trellow.api.Members;
using trellow.api.Members.Internal;

namespace trellow.api.Cards.Internal
{
	internal class CardsForMemberRequest : MembersRequest
	{
		public CardsForMemberRequest(IMemberId member, CardFilter filter)
			: base(member, "cards")
		{
			this.AddCommonCardParameters();
			this.AddFilter(filter);
		}
	}
}