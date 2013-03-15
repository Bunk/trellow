namespace trellow.api.Organizations.Internal
{
	internal class OrganizationsWithIdRequest : OrganizationsRequest
	{
		public OrganizationsWithIdRequest(string orgIdOrName) : base(orgIdOrName)
		{			
		}
	}
}