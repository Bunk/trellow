using System;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using trello.Services.Handlers;
using trellow.api;
using trellow.api.Cards;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public class ChecklistViewModel : ViewModelBase, 
        IHandle<CheckItemChanged>,
        IHandle<CheckItemCreated>,
        IHandle<CheckItemRemoved>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly Func<ChecklistItemViewModel> _itemFactory;
        private string _cardId;
        private string _name;
        private string _text;

        public string Id { get; private set; }

        [UsedImplicitly]
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

        [UsedImplicitly]
        public string Text
        {
            get { return _text; }
            set
            {
                if (value == _text) return;
                _text = value;
                NotifyOfPropertyChange(() => Text);
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
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            Items = new BindableCollection<ChecklistItemViewModel>();
        }

        public ChecklistViewModel InitializeWith(Card.Checklist list)
        {
            Id = list.Id;
            Name = list.Name;
            _cardId = list.IdCard;

            var items = list.CheckItems.Select(item => _itemFactory().InitializeWith(_cardId, Id, item));
            Items.Clear();
            Items.AddRange(items);

            return this;
        }

        [UsedImplicitly]
        public void AddItem(string text)
        {
            _eventAggregator.Publish(new CheckItemCreationRequested
            {
                ChecklistId = Id,
                Name = text
            });
            Text = null;
        }

        [UsedImplicitly]
        public bool CanAddItem(string text)
        {
            return !string.IsNullOrWhiteSpace(text);
        }

        [UsedImplicitly]
        public void Remove()
        {
            _eventAggregator.Publish(new ChecklistRemoved
            {
                CardId = _cardId,
                ChecklistId = Id
            });
        }

        public void Handle(CheckItemChanged message)
        {
            if (message.ChecklistId != Id) return;

            NotifyOfPropertyChange(() => ItemsChecked);
            NotifyOfPropertyChange(() => Items);
        }

        public void Handle(CheckItemCreated message)
        {
            if (message.ChecklistId != Id) return;

            var vm = _itemFactory().InitializeWith(_cardId, Id, message.CheckItem);
            Items.Add(vm);
        }

        public void Handle(CheckItemRemoved message)
        {
            if (message.ChecklistId != Id) return;

            var found = Items.Where(x => x.Id == message.CheckItemId).ToArray();
            foreach (var item in found)
                Items.Remove(item);

            NotifyOfPropertyChange(() => ItemsChecked);
        }
    }
}