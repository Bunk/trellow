using System;
using System.Linq;
using Caliburn.Micro;
using JetBrains.Annotations;
using TrelloNet;
using trello.Services.Handlers;
using trellow.api;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public class ChecklistViewModel : ViewModelBase, IHandle<CheckItemChanged>
    {
        private readonly Func<ChecklistItemViewModel> _itemFactory;
        private string _name;

        private string Id { get; set; }

        private string CardId { get; set; }

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

        public IObservableCollection<ChecklistItemViewModel> Items { get; private set; }

        public int ItemsChecked
        {
            get { return Items.Count(i => i.Checked); }
        }

        public ChecklistViewModel(ITrelloApiSettings settings,
                                  INavigationService navigation,
                                  IEventAggregator eventAggregator,
                                  Func<ChecklistItemViewModel> itemFactory) : base(settings, navigation)
        {
            _itemFactory = itemFactory;

            eventAggregator.Subscribe(this);

            Items = new BindableCollection<ChecklistItemViewModel>();
        }

        public ChecklistViewModel InitializeWith(Card.Checklist list)
        {
            Id = list.Id;
            Name = list.Name;
            CardId = list.IdCard;

            var items = list.CheckItems.Select(item => _itemFactory().InitializeWith(CardId, Id, item));
            Items.Clear();
            Items.AddRange(items);

            return this;
        }

        public void Handle(CheckItemChanged message)
        {
            NotifyOfPropertyChange(() => ItemsChecked);
            NotifyOfPropertyChange(() => Items);
        }
    }
}