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
        private string _name;
        private string _text;

        private string Id { get; set; }

        private string CardId { get; set; }

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
            CardId = list.IdCard;

            var items = list.CheckItems.Select(item => _itemFactory().InitializeWith(CardId, Id, item));
            Items.Clear();
            Items.AddRange(items);

            return this;
        }

        [UsedImplicitly]
        public void Add(string text)
        {
            _eventAggregator.Publish(new CheckItemCreationRequested
            {
                ChecklistId = Id,
                Name = text
            });
            Text = null;
        }

        [UsedImplicitly]
        public bool CanAdd(string text)
        {
            return !string.IsNullOrWhiteSpace(text);
        }

        public void Handle(CheckItemChanged message)
        {
            NotifyOfPropertyChange(() => ItemsChecked);
            NotifyOfPropertyChange(() => Items);
        }

        public void Handle(CheckItemCreated message)
        {
            if (message.ChecklistId != Id) return;

            var vm = _itemFactory().InitializeWith(CardId, Id, message.CheckItem);
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