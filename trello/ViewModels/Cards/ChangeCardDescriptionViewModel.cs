using System.Windows.Controls;
using Caliburn.Micro;
using JetBrains.Annotations;
using Strilanc.Value;
using trello.Assets;
using trello.Extensions;
using trello.Services.Messages;
using trello.Views.Cards;

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

        [UsedImplicitly]
        public void Accept()
        {
            _eventAggregator.Publish(new CardDescriptionChanged
            {
                CardId = CardId,
                Description = Description.Replace("\r", "\n")
            });
            TryClose();
        }

        [UsedImplicitly]
        public void TextChanged(ContentControl content)
        {
            content.Content
                   .MayCast<ChangeCardDescriptionView>()
                   .IfHasValueThenDo(view =>
                   {
                       // Make sure to automatically scroll the bottom of the text into view
                       // when we make a change to it.
                       view.ScrollViewer.UpdateLayout();
                       view.ScrollViewer.ScrollToVerticalOffset(view.Description.ActualHeight);
                   });
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