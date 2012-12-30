namespace trello.ViewModels
{
    public class ShellViewModel : PivotViewModel
    {
        private readonly BoardListViewModel _boards;
        private readonly CardListViewModel _cards;
        private readonly MessageListViewModel _messages;

        public ShellViewModel(BoardListViewModel boards,
                              CardListViewModel cards,
                              MessageListViewModel messages)
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
        }
    }
}