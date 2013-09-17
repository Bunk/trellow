using System;
using Caliburn.Micro;
using trello.Services.Messages;

namespace trello.ViewModels.Cards
{
    public class ChangeCardDueViewModel : DialogViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly string _cardId;

        public DateTime? Date { get; set; }

        public ChangeCardDueViewModel(object root, IEventAggregator eventAggregator, string cardId, DateTime? due) : base(root)
        {
            _eventAggregator = eventAggregator;
            _cardId = cardId;

            Date = due;
        }

        public void Confirm()
        {
            if (Date != null)
                _eventAggregator.Publish(new CardDueDateChanged { CardId = _cardId, DueDate = Date.Value });
            TryClose();
        }

        public void Remove()
        {
            _eventAggregator.Publish(new CardDueDateChanged { CardId = _cardId, DueDate = null });
            TryClose();
        }
    }
}