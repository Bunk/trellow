﻿using System;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using trello.Services;
using trello.Services.Messages;
using trello.Views.Cards;
using trellow.api;

namespace trello.ViewModels.Cards
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
        private readonly INavigationService _navigation;
        private readonly Func<CardDetailChecklistViewModel> _checklists;
        private readonly Func<CardDetailAttachmentsViewModel> _attachments;
        private readonly Func<CardDetailMembersViewModel> _members;
        private readonly IEventAggregator _eventAggregator;
        private readonly Func<CardDetailOverviewViewModel> _overview;
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
                                        IApplicationBar applicationBar,
                                        IEventAggregator eventAggregator,
                                        IWindowManager windowManager,
                                        Func<CardDetailOverviewViewModel> overview,
                                        Func<CardDetailChecklistViewModel> checklists,
                                        Func<CardDetailAttachmentsViewModel> attachments,
                                        Func<CardDetailMembersViewModel> members)
            : base(navigation, applicationBar)
        {
            _api = api;
            _navigation = navigation;
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
            var card = await _api.Cards.WithId(Id);
            if (card == null)
            {
                MessageBox.Show("The card could not be found.  Usually this means that someone else has removed it " +
                                "while you were browsing.");
                _navigation.GoBack();
                return;
            }

            Name = card.Name;

            Items.Add(_overview().Initialize(card).Bind(AppBar));
            Items.Add(_checklists().Initialize(card).Bind(AppBar));
            Items.Add(_attachments().Initialize(card).Bind(AppBar));
            Items.Add(_members().Initialize(card).Bind(AppBar));

            ActivateItem(Items[0]);
        }

        [UsedImplicitly]
        public void ChangeName()
        {
            var model = new ChangeCardNameViewModel(GetView(), _eventAggregator, Id)
            {
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