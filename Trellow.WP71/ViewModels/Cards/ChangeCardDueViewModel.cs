using System;
using Caliburn.Micro;
using JetBrains.Annotations;
using Trellow.Events;
using Trellow.UI;

namespace Trellow.ViewModels.Cards
{
    public class ChangeCardDueViewModel : DialogViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        [UsedImplicitly]
        public string CardId { get; set; }

        [UsedImplicitly]
        public DateTime? Date { get; set; }

        public ChangeCardDueViewModel(object root) : base(root)
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
        }

        [UsedImplicitly]
        public void Remove()
        {
            _eventAggregator.Publish(new CardDueDateChanged { CardId = CardId, DueDate = null });
            TryClose();
        }

        private void Accept()
        {
            if (Date != null)
                _eventAggregator.Publish(new CardDueDateChanged { CardId = CardId, DueDate = Date.Value });
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