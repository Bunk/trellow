using Caliburn.Micro;
using JetBrains.Annotations;
using TrelloNet;

namespace trello.ViewModels.Notifications
{
    public class AddedToBoardNotificationViewModel : NotificationViewModel
    {
        private readonly INavigationService _navigation;

        [UsedImplicitly]
        public BoardName Board { get; set; }

        public AddedToBoardNotificationViewModel(INavigationService navigation)
        {
            _navigation = navigation;
        }

        protected override NotificationViewModel Init(Notification dto)
        {
            base.Init(dto);

            var realDto = (AddedToBoardNotification) dto;

            Board = realDto.Data.Board;

            return this;
        }

        [UsedImplicitly]
        public void Navigate()
        {
            _navigation.UriFor<BoardViewModel>()
                       .WithParam(x => x.Id, Board.Id)
                       .Navigate();
        }
    }
}