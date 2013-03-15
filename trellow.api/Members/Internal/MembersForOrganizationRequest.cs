using trellow.api.Internal;
using trellow.api.Organizations;
using trellow.api.Organizations.Internal;

namespace trellow.api.Members.Internal
{
	internal class MembersForOrganizationRequest : OrganizationsRequest
	{
		public MembersForOrganizationRequest(IOrganizationId organization, MemberFilter filter)
			: base(organization, "members")
		{
			this.AddFilter(filter);
			this.AddRequiredMemberFields();
		}
	}
}