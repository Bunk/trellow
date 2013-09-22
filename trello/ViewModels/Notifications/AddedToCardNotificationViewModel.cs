using Caliburn.Micro;
using JetBrains.Annotations;
using trello.ViewModels.Cards;
using trellow.api;
using trellow.api.Notifications;

namespace trello.ViewModels.Notifications
{
    [UsedImplicitly]
	public class AddedToCardNotificationViewModel : NotificationViewModel
	{
        private readonly INavigationService _navigation;

        [UsedImplicitly]
		public BoardName Board { get; set; }

        [UsedImplicitly]
		public CardName Card { get; set; }

        public AddedToCardNotificationViewModel(INavigationService navigation)
        {
            _navigation = navigation;
        }

        protected override NotificationViewModel Init(Notification dto)
        {
            base.Init(dto);

            var x = (AddedToCardNotification) dto;
            Board = x.Data.Board;
            Card = x.Data.Card;

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