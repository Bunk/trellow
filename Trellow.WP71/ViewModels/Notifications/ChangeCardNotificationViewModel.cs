using Caliburn.Micro;
using JetBrains.Annotations;
using Trellow.ViewModels.Boards;
using Trellow.ViewModels.Cards;
using trellow.api;
using trellow.api.Notifications;

namespace Trellow.ViewModels.Notifications
{
    [UsedImplicitly]
	public class ChangeCardNotificationViewModel : NotificationViewModel
	{
        private readonly INavigationService _navigation;

        [UsedImplicitly]
        public CardName Card { get; set; }

        [UsedImplicitly]
        public BoardName Board { get; set; }

        public ChangeCardNotificationViewModel(INavigationService navigation)
        {
            _navigation = navigation;
        }

        protected override NotificationViewModel Init(Notification dto)
        {
            base.Init(dto);

            var x = (ChangeCardNotification) dto;
            Card = x.Data.Card;
            Board = x.Data.Board;

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

    [UsedImplicitly]
    public class CardMovedNotificationViewModel : NotificationViewModel
    {
        private readonly INavigationService _navigationService;

        [UsedImplicitly]
        public CardName Card { get; set; }

        [UsedImplicitly]
        public BoardName Board { get; set; }

        [UsedImplicitly]
        public ListName FromList { get; set; }

        [UsedImplicitly]
        public ListName ToList { get; set; }

        public CardMovedNotificationViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

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

        [UsedImplicitly]
        public void Navigate()
        {
            _navigationService.UriFor<BoardViewModel>()
                              .WithParam(x => x.Id, Board.Id)
                              .WithParam(x => x.SelectedListId, ToList.Id)
                              .Navigate();
        }
    }
}