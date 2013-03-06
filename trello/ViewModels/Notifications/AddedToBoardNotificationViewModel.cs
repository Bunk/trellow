using JetBrains.Annotations;
using TrelloNet;

namespace trello.ViewModels.Notifications
{
    public class AddedToBoardNotificationViewModel : NotificationViewModel
    {
        [UsedImplicitly]
        public BoardName Board { get; set; }

        protected override NotificationViewModel Init(Notification dto)
        {
            base.Init(dto);

            Board = ((AddedToBoardNotification) dto).Data.Board;

            return this;
        }
    }
}