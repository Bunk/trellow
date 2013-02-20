using Caliburn.Micro;
using JetBrains.Annotations;
using trello.Services.Handlers;

namespace trello.ViewModels
{
    public class ChangeCardNameViewModel : DialogViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private string _name;

        public string CardId { get; set; }

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

        public ChangeCardNameViewModel(object root, IEventAggregator eventAggregator) : base(root)
        {
            _eventAggregator = eventAggregator;
        }

        [UsedImplicitly]
        public void Accept()
        {
            _eventAggregator.Publish(new CardNameChanged
            {
                CardId = CardId, 
                Name = Name
            });
            TryClose();
        }
    }
}