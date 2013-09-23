using System;
using System.Linq;
using Caliburn.Micro;
using JetBrains.Annotations;
using trello.Services;
using trello.Services.Messages;
using trellow.api;
using trellow.api.Cards;
using trellow.api.Checklists;

namespace trello.ViewModels.Checklists
{
    [UsedImplicitly]
    public class ChecklistViewModel : ViewModelBase,
                                      IHandle<ChecklistNameChanged>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windows;
        private readonly Func<ChecklistItemViewModel> _itemFactory;
        private string _cardId;
        private string _name;
        private string _text;
        private int _itemsCount;
        private int _itemsChecked;

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

        [UsedImplicitly]
        public int ItemsCount
        {
            get { return _itemsCount; }
            set
            {
                if (value == _itemsCount) return;
                _itemsCount = value;
                NotifyOfPropertyChange(() => ItemsCount);
            }
        }

        [UsedImplicitly]
        public int ItemsChecked
        {
            get { return _itemsChecked; }
            set
            {
                if (value == _itemsChecked) return;
                _itemsChecked = value;
                NotifyOfPropertyChange(() => ItemsChecked);
            }
        }

        public IObservableCollection<ChecklistItemViewModel> Items { get; private set; }


        public ChecklistViewModel(IApplicationBar applicationBar,
                                  ITrelloApiSettings settings,
                                  INavigationService navigation,
                                  IEventAggregator eventAggregator,
                                  IWindowManager windows,
                                  Func<ChecklistItemViewModel> itemFactory) : base(applicationBar)
        {
            _itemFactory = itemFactory;
            _eventAggregator = eventAggregator;
            _windows = windows;
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

            UpdateCounts();

            return this;
        }

        private void UpdateCounts()
        {
            // note: The ProgressBar control has some bugs that screw you over, so no calculated properties, etc
            // http://geekswithblogs.net/cskardon/archive/2011/02/06/silverlight-progressbar-issues-with-binding.aspx
            ItemsChecked = 0;
            ItemsCount = Items.Count;
            ItemsChecked = Items.Count(item => item.Checked);
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

        [UsedImplicitly]
        public void Rename()
        {
            var vm = new RenameChecklistViewModel(GetView(), _eventAggregator, Id)
            {
                Name = Name
            };
            _windows.ShowDialog(vm);
        }

        public void AddItem(CheckItem item)
        {
            var vm = _itemFactory().InitializeWith(_cardId, Id, item);
            Items.Add(vm);

            UpdateCounts();
        }

        public void RemoveItem(string id)
        {
            var removals = Items.Where(item => item.Id == id).ToList();
            Items.RemoveRange(removals);

            UpdateCounts();
        }

        public void UpdateItem(string id, bool value)
        {
            Items.Where(x => x.Id == id).ToList()
                 .ForEach(model => { model.Checked = value; });

            UpdateCounts();
        }

        public void Handle(ChecklistNameChanged message)
        {
            if (message.ChecklistId != Id) return;

            Name = message.Name;
        }
    }
}