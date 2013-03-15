using RestSharp;
using trellow.api.Internal;

namespace trellow.api.Members.Internal
{
	internal class MembersRequest : RestRequest
	{
		public MembersRequest(IMemberId member, string resource = "")
			: base("members/{memberIdOrUsername}/" + resource)
		{
			Guard.NotNull(member, "member");
			AddParameter("memberIdOrUsername", member.GetMemberId(), ParameterType.UrlSegment);
		}

		public MembersRequest(string memberIdOrUsername, string resource = "")
			: this(new MemberId(memberIdOrUsername), resource)
		{
		}
	}
}