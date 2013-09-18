using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using JetBrains.Annotations;
using LinqToVisualTree;
using Microsoft.Phone.Shell;
using Strilanc.Value;
using trello.Assets;
using trello.Extensions;
using trello.Interactions;
using trello.Services.Messages;
using trello.ViewModels.Cards;
using trello.Views.Boards;
using trellow.api;
using trellow.api.Cards;
using trellow.api.Lists;

namespace trello.ViewModels.Boards
{
    [UsedImplicitly]
    public class BoardListViewModel : PivotItemViewModel,
                                      IConfigureTheAppBar,
                                      IHandle<CardCreated>,
                                      IHandle<CardDeleted>,
                                      IHandle<CardMovedToList>,
                                      IHandle<CardMovedToBoard>,
                                      IHandle<CardNameChanged>,
                                      IHandle<CardDescriptionChanged>,
                                      IHandle<CardDueDateChanged>,
                                      IHandle<CardCommented>,
                                      IHandle<CardLabelAdded>,
                                      IHandle<CardLabelRemoved>,
                                      IHandle<CardDetailChecklistViewModel.AggregationsUpdated>
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

        private CardViewModel CreateCardViewModel(Card card)
        {
            var vm = _cardFactory()
                .InitializeWith(card)
                .EnableInteractions(_interactionManager);

//            vm.ConductWith(this);
//
//            if (IsActive)
//                ((IActivate)vm).Activate();

            return vm;
        }

        private May<CardViewModel> FindCardViewModel(string id)
        {
            return Cards.Where(c => c.Id == id).MayFirst();
        }

        private async void RefreshLists()
        {
            var cards = await _api.Cards.ForList(new ListId(Id));
            var vms = cards.Select(CreateCardViewModel);

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
            existing.AddButton("add card", new AssetUri("Icons/dark/appbar.add.rest.png"), AddCard);

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
            if (message.Card.IdBoard != BoardId || message.Card.IdList != Id)
                return;

            var vm = CreateCardViewModel(message.Card);
            Cards.Add(vm);

            _navigation.UriFor<CardDetailPivotViewModel>()
                       .WithParam(x => x.Id, vm.Id)
                       .Navigate();
        }

        public void Handle(CardDeleted message)
        {
            FindCardViewModel(message.CardId)
                .IfHasValueThenDo(c => Cards.Remove(c));
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

        public void Handle(CardMovedToBoard message)
        {
            FindCardViewModel(message.CardId)
                .IfHasValueThenDo(card => Cards.Remove(card))
                .ElseDo(async () =>
                {
                    if (message.ListId != Id || message.BoardId != BoardId)
                        return;

                    // we want to add a new card here
                    // todo: Could possibly make the card initialize itself
                    var card = await _api.Cards.WithId(message.CardId);
                    var vm = CreateCardViewModel(card);
                    Cards.Add(vm);
                });
        }

        public void Handle(CardNameChanged message)
        {
            FindCardViewModel(message.CardId)
                .IfHasValueThenDo(card => card.Name = message.Name);
        }

        public void Handle(CardDescriptionChanged message)
        {
            FindCardViewModel(message.CardId)
                .IfHasValueThenDo(card => card.Desc = message.Description);
        }

        public void Handle(CardDueDateChanged message)
        {
            FindCardViewModel(message.CardId)
                .IfHasValueThenDo(card => card.Due = message.DueDate);
        }

        public void Handle(CardCommented message)
        {
            FindCardViewModel(message.CardId)
                .IfHasValueThenDo(card => card.Comments++);
        }

        public void Handle(CardDetailChecklistViewModel.AggregationsUpdated message)
        {
            FindCardViewModel(message.CardId)
                .IfHasValueThenDo(card =>
                {
                    card.CheckItems = message.CheckItemsCount;
                    card.CheckItemsChecked = message.CheckItemsCheckedCount;
                });
        }

        public void Handle(CardLabelAdded message)
        {
            FindCardViewModel(message.CardId)
                .IfHasValueThenDo(card => card.Labels.Add(new LabelViewModel(message.Color.ToString(), message.Name)));
        }

        public void Handle(CardLabelRemoved message)
        {
            FindCardViewModel(message.CardId)
                .IfHasValueThenDo(card =>
                {
                    var removing = card.Labels.Where(lbl => lbl.Color == message.Color.ToString());
                    card.Labels.RemoveRange(removing.ToArray());
                });
        }
    }
}