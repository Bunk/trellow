namespace trellow.api.Notifications
{
	public class AddAdminToBoardNotification : Notification
	{
		public NotificationData Data { get; set; }

		public class NotificationData
		{
			public BoardName Board { get; set; }
		}	
	}
}