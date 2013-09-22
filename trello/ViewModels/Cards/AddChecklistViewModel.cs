using Caliburn.Micro;
using JetBrains.Annotations;
using trello.Services.Handlers;
using trello.Services.Messages;

namespace trello.ViewModels.Cards
{
    public class AddChecklistViewModel : DialogViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private string _name;

        [UsedImplicitly]
        public string CardId { get; set; }

        [UsedImplicitly]
        public string BoardId { get; set; }

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
                Name = Name
            });
            TryClose();
        }
    }
}