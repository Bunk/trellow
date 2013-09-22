using System.Linq;
using Caliburn.Micro;
using JetBrains.Annotations;
using trellow.api.Cards;

namespace trello.ViewModels.Cards
{
    [UsedImplicitly]
    public sealed class CardDetailAttachmentsViewModel : PivotItemViewModel<CardDetailAttachmentsViewModel>
    {
        public IObservableCollection<AttachmentViewModel> Attachments { get; set; }

        public CardDetailAttachmentsViewModel()
        {
            DisplayName = "attachments";

            Attachments = new BindableCollection<AttachmentViewModel>();
        }

        public CardDetailAttachmentsViewModel Initialize(Card card)
        {
            var cover = card.IdAttachmentCover;
            var atts = card.Attachments.Select(att => new AttachmentViewModel().InitializeWith(att, cover));
            Attachments.Clear();
            Attachments.AddRange(atts);

            return this;
        }
    }
}
