using Caliburn.Micro;
using trellow.api;

namespace trello.ViewModels
{
    public class ShellViewModel : PivotViewModel
    {
        private readonly MyBoardsViewModel _myBoards;
        private readonly MyCardsViewModel _myCards;
        private readonly MessageListViewModel _messages;
        private string _subtitle;
        private string _title;

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
                              MyBoardsViewModel myBoards,
                              MyCardsViewModel myCards,
                              MessageListViewModel messages)
            : base(settings, navigation)
        {
            _myBoards = myBoards;
            _myCards = myCards;
            _messages = messages;

            settings.Username;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Items.Add(_myBoards);
            Items.Add(_myCards);
            Items.Add(_messages);

            ActivateItem(_myBoards);

            // remove back navigation to the splash page
            Navigation.RemoveBackEntry();
        }
    }
}