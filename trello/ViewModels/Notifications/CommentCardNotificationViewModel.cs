using JetBrains.Annotations;
using TrelloNet;

namespace trello.ViewModels.Notifications
{
    [UsedImplicitly]
	public class CommentCardNotificationViewModel : NotificationViewModel
	{
        [UsedImplicitly]
		public CardName Card { get; set; }

        [UsedImplicitly]
		public BoardName Board { get; set; }

        [UsedImplicitly]
        public string Text { get; set; }

        protected override NotificationViewModel Init(Notification dto)
        {
            base.Init(dto);

            var x = (CommentCardNotification) dto;
            Card = x.Data.Card;
            Board = x.Data.Board;
            Text = x.Data.Text;

            return this;
        }
	}
}