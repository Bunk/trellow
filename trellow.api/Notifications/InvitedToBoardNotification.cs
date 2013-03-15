namespace trellow.api.Notifications
{
	public class InvitedToBoardNotification : Notification
	{
		public NotificationData Data { get; set; }

		public class NotificationData
		{
			public BoardName Board { get; set; }
		}
	}
}