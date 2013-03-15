using trellow.api.Organizations;

namespace trellow.api.Notifications
{
	public class OrganizationName : IOrganizationId
	{
		public string Id { get; set; }
		public string Name { get; set; }

		public string GetOrganizationId()
		{
			return Id;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}