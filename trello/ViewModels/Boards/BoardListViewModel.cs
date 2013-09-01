using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using JetBrains.Annotations;
using LinqToVisualTree;
using Microsoft.Phone.Shell;
using trello.Assets;
using trello.Interactions;
using trello.Services.Handlers;
using trello.Services.Messages;
using trello.Views.Boards;
using trellow.api;
using trellow.api.Lists;

namespace trello.ViewModels.Boards
{
    [UsedImplicitly]
    public class BoardListViewModel : PivotItemViewModel,
                                      IConfigureTheAppBar,
                                      IHandle<CardCreated>,
                                      IHandle<CardDeleted>,
        IHandle<CardMovedToList>
    {
        private readonly ITrello _api;
        private readonly INavigationService _navigation;
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windows;
        private readonly Func<CardViewModel> _cardFactory;
        private string _name;
        private string _id;
        private string _previousId;
        private string _nextId;
        private string _boardId;
        private InteractionManager _interactionManager;

        [UsedImplicitly]
        public string Id
        {
            get { return _id; }
            set
            {
                if (value == _id) return;
                _id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        [UsedImplicitly]
        public string PreviousId
        {
            get { return _previousId; }
            set
            {
                if (value == _previousId) return;
                _previousId = value;
                NotifyOfPropertyChange(() => PreviousId);
            }
        }

        [UsedImplicitly]
        public string NextId
        {
            get { return _nextId; }
            set
            {
                if (value == _nextId) return;
                _nextId = value;
                NotifyOfPropertyChange(() => NextId);
            }
        }

        [UsedImplicitly]
        public string BoardId
        {
            get { return _boardId; }
            set
            {
                if (value == _boardId) return;
                _boardId = value;
                NotifyOfPropertyChange(() => BoardId);
            }
        }

        [UsedImplicitly]
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                NotifyOfPropertyChange(() => Name);

                DisplayName = _name;
            }
        }

        [UsedImplicitly]
        public IObservableCollection<CardViewModel> Cards { get; set; }

        public BoardListViewModel(ITrello api,
                                  INavigationService navigation,
                                  IEventAggregator events,
                                  IWindowManager windows,
                                  Func<CardViewModel> cardFactory)
        {
            _api = api;
            _navigation = navigation;
            _events = events;
            _windows = windows;
            _cardFactory = cardFactory;

            _events.Subscribe(this);

            Cards = new BindableCollection<CardViewModel>();
        }

        protected override void OnInitialize()
        {
            RefreshLists();

            _interactionManager = new InteractionManager();

            var view = GetView() as BoardListView;
            if (view == null) return;

            var scrollViewer = view.Cards.Descendants<ScrollViewer>().Cast<ScrollViewer>().SingleOrDefault();

            _interactionManager = new HoldCardInteraction(view.DragImage, view.Cards, scrollViewer);
            _interactionManager.AddInteraction(new DragVerticalInteraction(view.DragImage,
                                                                           view.Cards,
                                                                           scrollViewer,
                                                                           _events));
            _interactionManager.AddInteraction(new DragHorizontalInteraction(view.Cards,
                                                                             _events,
                                                                             PreviousId,
                                                                             NextId));
        }

        private async void RefreshLists()
        {
            var cards = await _api.Cards.ForList(new ListId(Id));
            var vms = cards.Select(card =>
            {
                var vm = _cardFactory()
                    .InitializeWith(card)
                    .EnableInteractions(_interactionManager);
                return vm;
            });

            Cards.Clear();
            Cards.AddRange(vms);
        }

        public BoardListViewModel InitializeWith(IEnumerable<List> allLists, List list)
        {
            Id = list.Id;
            BoardId = list.IdBoard;
            Name = list.Name;

            // we're going to cheat and use the BCL for a circular list
            var ll = new LinkedList<List>(allLists);
            var node = ll.Find(list);
            if (node != null) PreviousId = (node.Previous ?? ll.Last).Value.Id;
            if (node != null) NextId = (node.Next ?? ll.First).Value.Id;

            return this;
        }

        public ApplicationBar Configure(ApplicationBar existing)
        {
            var addButton = new ApplicationBarIconButton(new AssetUri("Icons/dark/appbar.add.rest.png"))
            {Text = "add card"};
            addButton.Click += (sender, args) => AddCard();
            existing.Buttons.Add(addButton);

            return existing;
        }

        private void AddCard()
        {
            var model = new AddCardViewModel(GetView(), _events)
            {
                BoardId = BoardId,
                ListId = Id
            };
            _windows.ShowDialog(model);
        }

        public void Handle(CardCreated message)
        {
            var vm = _cardFactory()
                .InitializeWith(message.Card)
                .EnableInteractions(_interactionManager);
            Cards.Add(vm);

            _navigation.UriFor<CardDetailPivotViewModel>()
                       .WithParam(x => x.Id, vm.Id)
                       .Navigate();
        }

        public void Handle(CardDeleted message)
        {
            // note: make sure this is idempotent since it will be called for each list on an opened board
            var found = Cards.Where(card => card.Id == message.CardId).ToArray();
            foreach (var card in found)
                Cards.Remove(card);
        }

        public void Handle(CardMovedToList message)
        {
            if (message.DestinationListId == Id)
            {
                // this list has a new card assigned to it from a
                // previous list
                var vm = _cardFactory()
                    .InitializeWith(message.Card)
                    .EnableInteractions(_interactionManager);

                Cards.Insert(0, vm);
                Cards.Refresh();
            }
            else if (message.SourceListId == Id)
            {
                // this list has lost a card assigned to another
                // list
                var vm = Cards
                    .Single(c => c.Id == message.Card.Id)
                    .DisableInteractions();

                //Cards.Remove(vm);
                //Cards.Refresh();
            }
        }
    }
}