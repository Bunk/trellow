﻿using System.Linq;
using Caliburn.Micro;
using JetBrains.Annotations;
using TrelloNet;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public sealed class CardDetailAttachmentsViewModel : PivotItemViewModel
    {
        public IObservableCollection<AttachmentViewModel> Attachments { get; set; }

        public CardDetailAttachmentsViewModel()
        {
            DisplayName = "attachments";

            Attachments = new BindableCollection<AttachmentViewModel>();
        }

        public CardDetailAttachmentsViewModel Initialize(Card card)
        {
            var atts = card.Attachments.Select(att => new AttachmentViewModel().InitializeWith(att));
            Attachments.Clear();
            Attachments.AddRange(atts);

            return this;
        }
    }
}