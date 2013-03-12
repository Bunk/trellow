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

    [UsedImplicitly]
    public class CardMovedNotificationViewModel : NotificationViewModel
    {
        [UsedImplicitly]
        public CardName Card { get; set; }

        [UsedImplicitly]
        public BoardName Board { get; set; }

        [UsedImplicitly]
        public ListName FromList { get; set; }

        [UsedImplicitly]
        public ListName ToList { get; set; }

        protected override NotificationViewModel Init(Notification dto)
        {
            base.Init(dto);

            var x = (CardMovedNotification)dto;
            Card = x.Data.Card;
            Board = x.Data.Board;
            FromList = x.Data.ListBefore;
            ToList = x.Data.ListAfter;

            return this;
        }
    }
}