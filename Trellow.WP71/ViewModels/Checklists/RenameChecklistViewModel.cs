using Caliburn.Micro;
using JetBrains.Annotations;
using Trellow.Events;

namespace Trellow.ViewModels.Checklists
{
    [UsedImplicitly]
    public class RenameChecklistViewModel : DialogViewModel
    {
        private readonly IEventAggregator _events;
        private readonly string _checklistId;
        private string _checklistName;

        [UsedImplicitly]
        public string ChecklistName
        {
            get { return _checklistName; }
            set
            {
                if (value == _checklistName) return;
                _checklistName = value;
                NotifyOfPropertyChange(() => ChecklistName);
            }
        }

        public RenameChecklistViewModel(object root, IEventAggregator events, string checklistId) : base(root)
        {
            _events = events;
            _checklistId = checklistId;
        }

        [UsedImplicitly]
        public void Accept()
        {
            _events.Publish(new ChecklistNameChanged
            {
                ChecklistId = _checklistId,
                Name = ChecklistName
            });
            TryClose();
        }
    }
}
