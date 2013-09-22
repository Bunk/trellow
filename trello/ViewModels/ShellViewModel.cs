using Caliburn.Micro;
using JetBrains.Annotations;
using trello.Services;
using trellow.api;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public class ShellViewModel : PivotViewModel
    {
        private readonly MyBoardsViewModel _myBoards;
        private readonly MyCardsViewModel _myCards;
        private readonly MyNotificationsViewModel _myNotifications;
        private string _subtitle;
        private string _title;

        [UsedImplicitly]
        public string Title
        {
            get { return _title; }
            set
            {
                if (value == _title) return;
                _title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        [UsedImplicitly]
        public string Subtitle
        {
            get { return _subtitle; }
            set
            {
                if (value == _subtitle) return;
                _subtitle = value;
                NotifyOfPropertyChange(() => Subtitle);
            }
        }

        public ShellViewModel(ITrelloApiSettings settings,
                              INavigationService navigation,
                              IApplicationBar applicationBar,
                              MyBoardsViewModel myBoards,
                              MyCardsViewModel myCards,
                              MyNotificationsViewModel myNotifications)
            : base(navigation, applicationBar)
        {
            _myBoards = myBoards;
            _myCards = myCards;
            _myNotifications = myNotifications;

            _myBoards.Bind(applicationBar);
            _myCards.Bind(applicationBar);
            _myNotifications.Bind(applicationBar);

            Title = "TRELLOW";
            Subtitle = settings.Fullname;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Items.Add(_myBoards);
            Items.Add(_myCards);
            Items.Add(_myNotifications);

            ActivateItem(_myBoards);

            // remove back navigation to the splash page
            Navigation.RemoveBackEntry();
        }
    }
}