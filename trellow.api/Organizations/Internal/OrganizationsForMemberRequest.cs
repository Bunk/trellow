using trellow.api.Internal;
using trellow.api.Members;
using trellow.api.Members.Internal;

namespace trellow.api.Organizations.Internal
{
	internal class OrganizationsForMemberRequest : MembersRequest
	{
		public OrganizationsForMemberRequest(IMemberId member, OrganizationFilter filter)
			: base(member, "organizations")
		{
			this.AddFilter(filter);
		}
	}
}