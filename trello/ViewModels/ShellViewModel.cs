using Caliburn.Micro;

namespace trello.ViewModels
{
    public class ShellViewModel : PivotViewModel
    {
        private readonly BoardListViewModel _boards;
        private readonly CardListViewModel _cards;
        private readonly MessageListViewModel _messages;
        private readonly INavigationService _navigationService;

        public ShellViewModel(BoardListViewModel boards,
                              CardListViewModel cards,
                              MessageListViewModel messages,
            INavigationService navigationService)
        {
            _boards = boards;
            _cards = cards;
            _messages = messages;
            _navigationService = navigationService;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Items.Add(_boards);
            Items.Add(_cards);
            Items.Add(_messages);

            ActivateItem(_boards);

            _navigationService.RemoveBackEntry();
        }
    }
}