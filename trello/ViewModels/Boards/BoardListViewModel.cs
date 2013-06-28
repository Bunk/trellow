using System;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using JetBrains.Annotations;
using LinqToVisualTree;
using Microsoft.Phone.Shell;
using trello.Assets;
using trello.Interactions;
using trello.Services.Handlers;
using trello.Views.Boards;
using trellow.api;
using trellow.api.Lists;

namespace trello.ViewModels.Boards
{
    [UsedImplicitly]
    public class BoardListViewModel : PivotItemViewModel,
                                      IConfigureTheAppBar,
                                      IHandle<CardCreated>,
                                      IHandle<CardDeleted>
    {
        private readonly ITrello _api;
        private readonly INavigationService _navigation;
        private readonly IEventAggregator _events;
        private readonly IWindowManager _windows;
        private readonly Func<CardViewModel> _cardFactory;
        private string _name;
        private string _id;
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
            _interactionManager.AddInteraction(new DragVerticalInteraction(view.DragImage, view.Cards, scrollViewer,
                                                                           _events));
            _interactionManager.AddInteraction(new DragHorizontalInteraction(view.DragImage, view.Cards, _events));
        }

        private async void RefreshLists()
        {
            var cards = await _api.Cards.ForList(new ListId(Id));
            var vms = cards.Select(card =>
            {
                var vm = _cardFactory().InitializeWith(card, _interactionManager);
                return vm;
            });

            Cards.Clear();
            Cards.AddRange(vms);
        }

        public BoardListViewModel InitializeWith(List list)
        {
            Id = list.Id;
            BoardId = list.IdBoard;
            Name = list.Name;

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
            var vm = _cardFactory().InitializeWith(message.Card);
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
    }
}