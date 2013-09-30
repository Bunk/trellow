using Caliburn.Micro;
using JetBrains.Annotations;
using Trellow.Events;

namespace Trellow.ViewModels.Cards
{
    public class AddChecklistViewModel : DialogViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private string _checklistName;

        [UsedImplicitly]
        public string CardId { get; set; }

        [UsedImplicitly]
        public string BoardId { get; set; }

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

        public AddChecklistViewModel(object root, IEventAggregator eventAggregator) : base(root)
        {
            _eventAggregator = eventAggregator;
        }

        public void Accept()
        {
            _eventAggregator.Publish(new ChecklistCreationRequested
            {
                CardId = CardId,
                BoardId = BoardId,
                Name = ChecklistName
            });
            TryClose();
        }
    }
}