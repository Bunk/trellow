using System;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using TrelloNet;
using trello.Services;
using trello.Services.Handlers;
using trello.Views;
using trellow.api;

namespace trello.ViewModels
{
    public class CardDetailPivotViewModel : PivotViewModel,
                                            IHandle<CardNameChanged>,
                                            IHandle<CardDetailPivotViewModel.PivotRequestingNavigation>
    {
        public enum Screen
        {
            Overview = 0,
            Checklists = 1,
            Attachments = 2,
            Members = 3
        }

        private readonly ITrello _api;
        private readonly Func<CardDetailChecklistViewModel> _checklists;
        private readonly Func<CardDetailAttachmentsViewModel> _attachments;
        private readonly Func<CardDetailMembersViewModel> _members;
        private readonly IEventAggregator _eventAggregator;
        private readonly Func<CardDetailOverviewViewModel> _overview;
        private readonly IProgressService _progress;
        private readonly IWindowManager _windowManager;
        private string _name;

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

        public CardDetailPivotViewModel(ITrello api,
                                        ITrelloApiSettings settings,
                                        INavigationService navigation,
                                        IEventAggregator eventAggregator,
                                        IProgressService progress,
                                        IWindowManager windowManager,
                                        Func<CardDetailOverviewViewModel> overview,
                                        Func<CardDetailChecklistViewModel> checklists,
            Func<CardDetailAttachmentsViewModel> attachments,
            Func<CardDetailMembersViewModel> members)
            : base(settings, navigation)
        {
            _api = api;
            _progress = progress;
            _windowManager = windowManager;
            _overview = overview;
            _checklists = checklists;
            _attachments = attachments;
            _members = members;

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
        }

        protected override async void OnInitialize()
        {
            _progress.Show("Loading...");

            var card = new Card();
            try
            {
                card = await _api.Async.Cards.WithId(Id);
            }
            catch
            {
                MessageBox.Show("Could not load this card.  Please ensure that you " +
                                "have an active internet connection.");
            }

            Name = card.Name;

            Items.Add(_overview().Initialize(card));
            Items.Add(_checklists().Initialize(card));
            Items.Add(_attachments().Initialize(card));
            Items.Add(_members().Initialize(card));

            ActivateItem(Items[0]);

            _progress.Hide();
        }

        [UsedImplicitly]
        public void ChangeName()
        {
            var model = new ChangeCardNameViewModel(GetView(), _eventAggregator)
            {
                CardId = Id,
                Name = Name
            };

            _windowManager.ShowDialog(model);
        }

        public void Handle(CardNameChanged message)
        {
            Name = message.Name;
        }

        public void Handle(PivotRequestingNavigation message)
        {
            UsingView<CardDetailPivotView>(view => view.Items.SelectedIndex = (int) message.Screen);
        }

        public class PivotRequestingNavigation
        {
            public Screen Screen { get; set; }
        }
    }
}