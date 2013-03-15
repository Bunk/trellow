using trellow.api.Actions;

namespace trellow.api.Notifications
{
	public class ChangeCardNotification : Notification
	{
        public ChangeCardNotification()
        {
            Data = new NotificationData();
        }

		public NotificationData Data { get; set; }

		public class NotificationData : IUpdateData
		{
			public CardName Card { get; set; }
			public BoardName Board { get; set; }
		    public Old Old { get; set; }
		}
	}

    public class CardMovedNotification : Notification
    {
        public CardMovedNotification()
        {
            Data = new NotificationData();
        }

        public NotificationData Data { get; set; }

        public class NotificationData : IUpdateData
        {
            public CardName Card { get; set; }
            public BoardName Board { get; set; }
            public ListName ListBefore { get; set; }
            public ListName ListAfter { get; set; }
            public Old Old { get; set; }
        }
    }
}