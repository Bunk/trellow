using JetBrains.Annotations;
using trellow.api;
using trellow.api.Notifications;

namespace trello.ViewModels.Notifications
{
    [UsedImplicitly]
	public class CloseBoardNotificationViewModel : NotificationViewModel
	{
        [UsedImplicitly]
		public BoardName Board { get; set; }

        protected override NotificationViewModel Init(Notification dto)
        {
            base.Init(dto);

            Board = ((CloseBoardNotification) dto).Data.Board;

            return this;
        }
	}
}