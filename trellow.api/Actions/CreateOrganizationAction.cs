using trellow.api.Notifications;

namespace trellow.api.Actions
{
	public class CreateOrganizationAction : Action
	{
		public ActionData Data { get; set; }

		public class ActionData
		{
			public OrganizationName Organization { get; set; }
		}
	}
}