using System;
using Caliburn.Micro;
using trello.Services;
using trello.Services.Data;

namespace trello.ViewModels
{
    public class CardDetailViewModel : PivotViewModel
    {
        private readonly ICardService _cardService;
        private readonly Func<CardDetailOverviewViewModel> _overviewFactory;
        private string _name;
        private string _boardName;

        public string Id { get; set; }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

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

        public CardDetailViewModel(ITrelloSettings settings,
                                   INavigationService navigation,
                                   ICardService cardService,
                                   Func<CardDetailOverviewViewModel> overviewFactory) : base(settings, navigation)
        {
            _cardService = cardService;
            _overviewFactory = overviewFactory;
        }

        protected override async void OnInitialize()
        {
            var card = await _cardService.WithId(Id);
            Name = card.Name;
            BoardName = card.Board.Name;

            var details = new CardDetailOverviewViewModel().InitializeWith(card);

            Items.Add(details);

            ActivateItem(Items[0]);
        }
    }
}