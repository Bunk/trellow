using RestSharp;
using trellow.api.Internal;
using trellow.api.Members;

namespace trellow.api.Cards.Internal
{
	internal class CardsAddMemberRequest : CardsRequest
	{
		public	CardsAddMemberRequest(ICardId card, IMemberId member) 
			: base(card, "members", Method.POST)
		{
			Guard.NotNull(member, "member");
			this.AddValue(member.GetMemberId());
		}
	}
}