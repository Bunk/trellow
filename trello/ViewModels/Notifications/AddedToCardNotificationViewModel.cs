using JetBrains.Annotations;
using TrelloNet;

namespace trello.ViewModels.Notifications
{
    [UsedImplicitly]
	public class AddedToCardNotificationViewModel : NotificationViewModel
	{
        [UsedImplicitly]
		public BoardName Board { get; set; }

        [UsedImplicitly]
		public CardName Card { get; set; }

        protected override NotificationViewModel Init(Notification dto)
        {
            base.Init(dto);

            var x = (AddedToCardNotification) dto;
            Board = x.Data.Board;
            Card = x.Data.Card;

            return this;
        }
	}
}