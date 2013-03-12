using JetBrains.Annotations;
using TrelloNet;

namespace trello.ViewModels.Notifications
{
    [UsedImplicitly]
	public class ChangeCardNotificationViewModel : NotificationViewModel
	{
        [UsedImplicitly]
        public CardName Card { get; set; }

        [UsedImplicitly]
        public BoardName Board { get; set; }

        protected override NotificationViewModel Init(Notification dto)
        {
            base.Init(dto);

            var x = (ChangeCardNotification) dto;
            Card = x.Data.Card;
            Board = x.Data.Board;

            return this;
        }
	}
}