using System.Collections.Generic;
using trellow.api.Internal;
using trellow.api.Organizations;
using trellow.api.Organizations.Internal;

namespace trellow.api.Actions.Internal
{
	internal class ActionsForOrganizationRequest : OrganizationsRequest
	{
		public ActionsForOrganizationRequest(IOrganizationId organization, ISince since, Paging paging, IEnumerable<ActionType> filter)
			: base(organization, "actions")
		{
			this.AddTypeFilter(filter);
			this.AddSince(since);
			this.AddPaging(paging);
		}
	}
}