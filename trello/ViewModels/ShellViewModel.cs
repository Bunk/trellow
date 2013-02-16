using Caliburn.Micro;
using trellow.api;

namespace trello.ViewModels
{
    public class ShellViewModel : PivotViewModel
    {
        private readonly MyBoardsViewModel _myBoards;
        private readonly MyCardsViewModel _myCards;
        private readonly MessageListViewModel _messages;

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