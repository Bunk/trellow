using Caliburn.Micro;
using trello.Assets;
using trello.Extensions;
using trello.Services.Messages;

namespace trello.ViewModels.Cards
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

        public ChangeCardDescriptionViewModel(object root) : base(root)
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
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

        protected override void OnActivate()
        {
            base.OnActivate();

            UpdateApplicationBar(bar =>
            {
                bar.AddButton("ok", new AssetUri("Icons/dark/appbar.check.rest.png"), Accept);
                bar.AddButton("cancel", new AssetUri("Icons/dark/appbar.close.rest.png"), TryClose);
            });
        }
    }
}
