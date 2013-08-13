using Caliburn.Micro;
using trello.Services.Handlers;
using trello.Services.Messages;

namespace trello.ViewModels
{
    public class ChangeCardDescriptionViewModel : DialogViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private string _description;

        public string CardId { get; set; }

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                _description = value;
                NotifyOfPropertyChange(() => Description);
            }
        }

        public ChangeCardDescriptionViewModel(object root, IEventAggregator eventAggregator) : base(root)
        {
            _eventAggregator = eventAggregator;
        }

        public void Accept()
        {
            _eventAggregator.Publish(new CardDescriptionChanged
            {
                CardId = CardId, 
                Description = Description.Replace("\r", "\n")
            });
            TryClose();
        }
    }
}
