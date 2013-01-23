using System;
using System.Linq;
using Caliburn.Micro;
using trellow.api;
using trellow.api.Models;

namespace trello.ViewModels
{
    public class ChecklistViewModel : ViewModelBase
    {
        private readonly Func<ChecklistItemViewModel> _itemFactory;

        public string Id { get; set; }

        public string IdBoard { get; set; }

        public string Name { get; set; }

        public IObservableCollection<ChecklistItemViewModel> Items { get; set; } 

        public ChecklistViewModel(ITrelloApiSettings settings, INavigationService navigation, Func<ChecklistItemViewModel> itemFactory) : base(settings, navigation)
        {
            _itemFactory = itemFactory;
            Items = new BindableCollection<ChecklistItemViewModel>();
        }

        public ChecklistViewModel For(CheckList checkList)
        {
            Items.Clear();

            Id = checkList.Id;
            IdBoard = checkList.IdBoard;
            Name = checkList.Name;

            Items.AddRange(checkList.CheckItems.Select(x => _itemFactory().For(x)));

            return this;
        }

        public void Toggle(ChecklistItemViewModel item)
        {
            item.Checked = !item.Checked;
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
    }
}
