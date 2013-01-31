using System;
using Caliburn.Micro;
using JetBrains.Annotations;
using trello.Views;
using trellow.api;
using trellow.api.Data;
using Strilanc.Value;
using trellow.api.Data.Services;

namespace trello.ViewModels
{
    [UsedImplicitly]
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
            card.IfHasValueThenDo(x =>
            {
                Name = x.Name;
                BoardName = x.Board.Name;

                Details = _overviewFactory().InitializeWith(x);
                Details.Parent = this;
            });
        }

        public void NavigateToScreen(int index)
        {
            UsingView<CardDetailShellView>(view =>
            {
                view.DetailPivot.SelectedIndex = index;
            });
        }
    }
}