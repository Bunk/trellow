using Caliburn.Micro;
using JetBrains.Annotations;
using Trellow.Events;

namespace Trellow.ViewModels.Boards
{
    public class AddCardViewModel : DialogViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private string _cardName;

        [UsedImplicitly]
        public string BoardId { get; set; }

        [UsedImplicitly]
        public string ListId { get; set; }

        [UsedImplicitly]
        public string CardName
        {
            get { return _cardName; }
            set
            {
                if (value == _cardName) return;
                _cardName = value;
                NotifyOfPropertyChange(() => CardName);
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
                Name = CardName,
                BoardId = BoardId,
                ListId = ListId
            });
            TryClose();
        }
    }
}
