using trellow.api.Internal;
using trellow.api.Organizations;
using trellow.api.Organizations.Internal;

namespace trellow.api.Boards.Internal
{
	internal class BoardsForOrganizationRequest : OrganizationsRequest
	{
		public BoardsForOrganizationRequest(IOrganizationId organization, BoardFilter filter)
			: base(organization, "boards")
		{
			this.AddFilter(filter);
		}
	}
}