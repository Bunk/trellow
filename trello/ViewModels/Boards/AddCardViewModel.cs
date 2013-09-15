using Caliburn.Micro;
using JetBrains.Annotations;
using trello.Services.Handlers;
using trello.Services.Messages;

namespace trello.ViewModels.Boards
{
    public class AddCardViewModel : DialogViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private string _name;

        public string BoardId { get; set; }

        public string ListId { get; set; }

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

        public AddCardViewModel(object root, IEventAggregator eventAggregator) : base(root)
        {
            _eventAggregator = eventAggregator;
        }

        [UsedImplicitly]
        public void Accept()
        {
            _eventAggregator.Publish(new CardCreationRequested
            {
                Name = Name,
                BoardId = BoardId,
                ListId = ListId
            });
            TryClose();
        }
    }
}
