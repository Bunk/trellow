using trellow.api.Tokens.Internal;

namespace trellow.api.Members.Internal
{
	internal class MembersForTokenRequest : TokensRequest
	{
		public MembersForTokenRequest(string token)
			: base(token, "member")
		{
		}
	}
}