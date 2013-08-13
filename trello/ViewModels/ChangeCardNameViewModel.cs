using Caliburn.Micro;
using JetBrains.Annotations;
using trello.Services.Handlers;
using trello.Services.Messages;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public class ChangeCardNameViewModel : DialogViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly string _cardId;
        private string _name;

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

        public ChangeCardNameViewModel(object root, IEventAggregator eventAggregator, string cardId) : base(root)
        {
            _eventAggregator = eventAggregator;
            _cardId = cardId;
        }

        [UsedImplicitly]
        public void Accept()
        {
            _eventAggregator.Publish(new CardNameChanged
            {
                CardId = _cardId, 
                Name = Name
            });
            TryClose();
        }
    }
}