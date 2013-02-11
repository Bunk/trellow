using Caliburn.Micro;
using JetBrains.Annotations;
using trellow.api.Models;

namespace trello.ViewModels
{
    [UsedImplicitly]
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