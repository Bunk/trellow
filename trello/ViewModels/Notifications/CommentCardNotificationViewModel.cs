using Caliburn.Micro;
using JetBrains.Annotations;
using trellow.api;
using trellow.api.Notifications;

namespace trello.ViewModels.Notifications
{
    [UsedImplicitly]
	public class CommentCardNotificationViewModel : NotificationViewModel
	{
        private readonly INavigationService _navigation;

        [UsedImplicitly]
		public CardName Card { get; set; }

        [UsedImplicitly]
		public BoardName Board { get; set; }

        [UsedImplicitly]
        public string Text { get; set; }

        public CommentCardNotificationViewModel(INavigationService navigation)
        {
            _navigation = navigation;
        }

        protected override NotificationViewModel Init(Notification dto)
        {
            base.Init(dto);

            var x = (CommentCardNotification) dto;
            Card = x.Data.Card;
            Board = x.Data.Board;
            Text = x.Data.Text;

            return this;
        }

        [UsedImplicitly]
        public void Navigate()
        {
            _navigation.UriFor<CardDetailPivotViewModel>()
                       .WithParam(x => x.Id, Card.Id)
                       .Navigate();
        }
	}
}