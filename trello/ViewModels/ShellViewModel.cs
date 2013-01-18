using Caliburn.Micro;
using Microsoft.Phone.Shell;
using trellow.api;

namespace trello.ViewModels
{
    public class ShellViewModel : PivotViewModel
    {
        private readonly BoardsViewModel _boards;
        private readonly CardListViewModel _cards;
        private readonly MessageListViewModel _messages;

        public ShellViewModel(ITrelloApiSettings settings,
                              INavigationService navigation,
                              BoardsViewModel boards,
                              CardListViewModel cards,
                              MessageListViewModel messages)
            : base(settings, navigation)
        {
            _boards = boards;
            _cards = cards;
            _messages = messages;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Items.Add(_boards);
            Items.Add(_cards);
            Items.Add(_messages);

            ActivateItem(_boards);

            // remove back navigation to the splash page
            Navigation.RemoveBackEntry();
        }

        protected override ApplicationBar BuildDefaultAppBar()
        {
            var bar = base.BuildDefaultAppBar();


            return bar;
        }
    }
}