using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using trellow.api;
using trellow.api.Models;

namespace trello.ViewModels
{
    public class ChecklistViewModel : ViewModelBase
    {
        private readonly Func<ChecklistItemViewModel> _itemFactory;
        
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

        public IObservableCollection<ChecklistItemViewModel> Items { get; set; }

        public int ItemsChecked
        {
            get { return Items.Count(i => i.Checked); }
        }

        public ChecklistViewModel(ITrelloApiSettings settings, INavigationService navigation, Func<ChecklistItemViewModel> itemFactory) : base(settings, navigation)
        {
            _itemFactory = itemFactory;
            Items = new BindableCollection<ChecklistItemViewModel>();
            Items.CollectionChanged += (sender, args) => NotifyOfPropertyChange(() => ItemsChecked);
        }

        public ChecklistViewModel For(CheckList checkList, IEnumerable<CheckItemState> checks)
        {
            Items.Clear();

            Id = checkList.Id;
            Name = checkList.Name;

            Items.AddRange(checkList.CheckItems.Select(x =>
            {
                var item = _itemFactory().For(x);
                
                // todo: This is the way that the Trello API seems to work currently--hopefully they fix it
                if (checks.Any(c => c.IdCheckItem == item.Id && c.State == CheckListItem.CheckState.Complete))
                    item.Checked = true;

                item.PropertyChanged += (sender, args) => CheckItemChanged();
                return item;
            }));

            return this;
        }

        public void CheckItemChanged()
        {
            NotifyOfPropertyChange(() => ItemsChecked);
        }
    }

    public class ChecklistItemViewModel : PropertyChangedBase
    {
        private string _id;
        private string _name;
        private bool _checked;

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

        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (value.Equals(_checked)) return;
                _checked = value;

                NotifyOfPropertyChange(() => Checked);
            }
        }

        public ChecklistItemViewModel For(CheckListItem item)
        {
            Id = item.Id;
            Name = item.Name;
            Checked = item.State == CheckListItem.CheckState.Complete;

            return this;
        }

        public void Toggle()
        {
            Checked = !Checked;
        }
    }
}
