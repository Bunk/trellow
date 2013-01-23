using System;
using Caliburn.Micro;
using trellow.api;
using trellow.api.Data;

namespace trello.ViewModels
{
    public class CardDetailShellViewModel : ViewModelBase
    {
        private readonly ICardService _cardService;
        private readonly Func<CardDetailViewModel> _overviewFactory;
        private string _name;
        private string _boardName;
        private CardDetailViewModel _details;

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

        public CardDetailViewModel Details
        {
            get { return _details; }
            set
            {
                if (Equals(value, _details)) return;
                _details = value;
                NotifyOfPropertyChange(() => Details);
            }
        }

        public CardDetailShellViewModel(ITrelloApiSettings settings,
                                        INavigationService navigation,
                                        ICardService cardService,
                                        Func<CardDetailViewModel> overviewFactory) : base(settings, navigation)
        {
            _cardService = cardService;
            _overviewFactory = overviewFactory;
        }

        protected override async void OnInitialize()
        {
            var card = await _cardService.WithId(Id);
            Name = card.Name;
            BoardName = card.Board.Name;

            var details = _overviewFactory().InitializeWith(card);
            Details = details;
        }
    }
}