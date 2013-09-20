using System;
using Caliburn.Micro;
using trello.Assets;
using trello.Extensions;
using trello.Services.Messages;

namespace trello.ViewModels.Cards
{
    public class ChangeCardDueViewModel : DialogViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly string _cardId;

        public string CardId { get; set; }

        public DateTime? Date { get; set; }

        public ChangeCardDueViewModel(object root) : base(root)
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
        }

        public void Accept()
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

        protected override void OnActivate()
        {
            base.OnActivate();

            UpdateApplicationBar(bar =>
            {
                bar.AddButton("accept", new AssetUri("Icons/dark/appbar.check.rest.png"), Accept);
                bar.AddButton("cancel", new AssetUri("Icons/dark/appbar.close.rest.png"), TryClose);
            });
        }
    }
}