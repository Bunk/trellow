using trellow.api.Models;

namespace trello.ViewModels
{
    public class CardDetailOverviewViewModel : PivotItemViewModel
    {
        private string _boardName;

        public string Id { get; set; }

        public string BoardName
        {
            get { return _boardName; }
            set
            {
                if (value == _boardName) return;
                _boardName = value;
                NotifyOfPropertyChange(() => BoardName);
            }
        }

        public CardDetailOverviewViewModel()
        {
            DisplayName = "overview";
        }

        public CardDetailOverviewViewModel InitializeWith(Card card)
        {
            Id = card.Id;
            BoardName = card.Board.Name;
            return this;
        }

        protected override void OnInitialize()
        {
        }
    }
}