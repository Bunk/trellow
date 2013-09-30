using Caliburn.Micro;
using JetBrains.Annotations;
using Trellow.Events;

namespace Trellow.ViewModels.Cards
{
    [UsedImplicitly]
    public class ChangeCardNameViewModel : DialogViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly string _cardId;
        private string _cardName;

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
                Name = CardName
            });
            TryClose();
        }
    }
}