using System;
using System.Linq;
using Caliburn.Micro;
using TrelloNet;
using trello.Services.Handlers;

namespace trello.ViewModels
{
    public sealed class CardDetailChecklistViewModel : PivotItemViewModel, IHandle<CheckItemChanged>
    {
        private readonly Func<ChecklistViewModel> _checklistFactory;
        private readonly IEventAggregator _eventAggregator;

        public CardDetailChecklistViewModel(IEventAggregator eventAggregator, Func<ChecklistViewModel> checklistFactory)
        {
            DisplayName = "checklists";

            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            _checklistFactory = checklistFactory;

            Checklists = new BindableCollection<ChecklistViewModel>();
        }

        public IObservableCollection<ChecklistViewModel> Checklists { get; set; }

        public void Handle(CheckItemChanged message)
        {
            var update = new ChecklistAggregationsUpdated
            {
                ChecklistCount = Checklists.Count,
                CheckItemsCount = Checklists.Aggregate(0, (i, model) => i + model.Items.Count),
                CheckItemsCheckedCount = Checklists.Aggregate(0, (i, model) => i + model.ItemsChecked)
            };
            _eventAggregator.Publish(update);
        }

        public CardDetailChecklistViewModel Initialize(Card card)
        {
            var checks = card.Checklists.Select(list => _checklistFactory().InitializeWith(list));
            Checklists.Clear();
            Checklists.AddRange(checks);

            return this;
        }

        public class ChecklistAggregationsUpdated
        {
            public int ChecklistCount { get; set; }

            public int CheckItemsCount { get; set; }

            public int CheckItemsCheckedCount { get; set; }
        }
    }
}