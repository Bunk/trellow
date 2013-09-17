using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Shell;
using trello.Assets;
using trello.Extensions;
using trello.Services.Messages;
using trello.ViewModels.Cards;
using trellow.api.Cards;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public sealed class CardDetailChecklistViewModel : PivotItemViewModel,
                                                       IConfigureTheAppBar,
                                                       IHandle<CheckItemChanged>,
                                                       IHandle<CheckItemCreated>,
                                                       IHandle<CheckItemRemoved>,
                                                       IHandle<ChecklistCreated>,
                                                       IHandle<ChecklistRemoved>
    {
        private readonly Func<ChecklistViewModel> _checklistFactory;
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _window;
        private string _boardId;
        private string _cardId;

        [UsedImplicitly]
        public IObservableCollection<ChecklistViewModel> Checklists { get; set; }

        public CardDetailChecklistViewModel(IEventAggregator eventAggregator,
                                            IWindowManager window,
                                            Func<ChecklistViewModel> checklistFactory)
        {
            DisplayName = "checklists";

            _eventAggregator = eventAggregator;
            _window = window;
            _checklistFactory = checklistFactory;

            Checklists = new BindableCollection<ChecklistViewModel>();
        }

        protected override void OnActivate()
        {
            _eventAggregator.Subscribe(this);
        }

        protected override void OnDeactivate(bool close)
        {
            _eventAggregator.Unsubscribe(this);
        }

        public CardDetailChecklistViewModel Initialize(Card card)
        {
            _cardId = card.Id;
            _boardId = card.IdBoard;

            var checks = card.Checklists.Select(list => _checklistFactory().InitializeWith(list));
            Checklists.Clear();
            Checklists.AddRange(checks);

            return this;
        }

        public ApplicationBar Configure(ApplicationBar existing)
        {
            return existing.AddButton("add list", new AssetUri("/Icons/dark/appbar.add.rest.png"), Add);
        }

        [UsedImplicitly]
        public void Add()
        {
            var vm = new AddChecklistViewModel(GetView(), _eventAggregator)
            {
                BoardId = _boardId,
                CardId = _cardId
            };
            _window.ShowDialog(vm);
        }

        public class AggregationsUpdated
        {
            public string CardId { get; set; }

            public int ChecklistCount { get; set; }

            public int CheckItemsCount { get; set; }

            public int CheckItemsCheckedCount { get; set; }
        }

        private void PublishAggregations()
        {
            var update = new AggregationsUpdated
            {
                CardId = _cardId,
                ChecklistCount = Checklists.Count,
                CheckItemsCount = Checklists.Aggregate(0, (i, model) => i + model.Items.Count),
                CheckItemsCheckedCount = Checklists.Aggregate(0, (i, model) => i + model.ItemsChecked)
            };
            _eventAggregator.Publish(update);
        }

        public void Handle(CheckItemChanged message)
        {
            if (message.CardId != _cardId)
                return;

            PublishAggregations();
        }

        public void Handle(CheckItemCreated message)
        {
            // bug: this currently runs before the items are actually updated
            // probably want to send the aggregations for the particular list in 
            // the message
            if (Checklists.All(list => list.Id != message.ChecklistId))
                return;

            PublishAggregations();
        }

        public void Handle(CheckItemRemoved message)
        {
            // bug: this currently runs before the items are actually updated
            // probably want to send the aggregations for the particular list in 
            // the message
            if (Checklists.All(list => list.Id != message.ChecklistId))
                return;

            PublishAggregations();
        }

        public void Handle(ChecklistCreated message)
        {
            if (message.CardId != _cardId) return;

            var checklist = new Card.Checklist
            {
                IdBoard = _boardId,
                IdCard = _cardId,
                Id = message.ChecklistId,
                Name = message.Name,
                CheckItems = new List<Card.CheckItem>()
            };
            var list = _checklistFactory().InitializeWith(checklist);
            Checklists.Insert(0, list);

            PublishAggregations();
        }

        public void Handle(ChecklistRemoved message)
        {
            if (message.CardId != _cardId) 
                return;

            var found = Checklists.Where(x => x.Id == message.ChecklistId).ToArray();
            foreach (var list in found)
                Checklists.Remove(list);

            PublishAggregations();
        }
    }
}