using System;
using Caliburn.Micro;
using trellow.api;
using trellow.api.Data;

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

        public CardDetailViewModel(ITrelloApiSettings settings,
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

            var details = _overviewFactory().InitializeWith(card);
            var checklists = new CardDetailChecklistsViewModel();
            var attachments = new CardDetailAttachmentsViewModel();
            var members = new CardDetailMembersViewModel();
            var activity = new CardDetailActivityViewModel();

            Items.Add(details);
            Items.Add(checklists);
            Items.Add(attachments);
            Items.Add(members);
            Items.Add(activity);

            ActivateItem(Items[0]);
        }
    }
}