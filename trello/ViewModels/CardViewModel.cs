using System;
using System.Linq;
using Caliburn.Micro;
using trellow.api.Models;

namespace trello.ViewModels
{
    public class CardViewModel : Screen
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public DateTime? Due { get; set; }

        public int Votes { get; set; }

        public int Comments { get; set; }

        public int CheckItems { get; set; }

        public int CheckItemsChecked { get; set; }

        public int Attachments { get; set; }

        public string CoverUri { get; set; }

        public int CoverHeight { get; set; }

        public int CoverWidth { get; set; }

        public IObservableCollection<MemberViewModel> Members { get; set; }

        public IObservableCollection<LabelViewModel> Labels { get; set; }

        public CardViewModel()
        {
            Members = new BindableCollection<MemberViewModel>();
            Labels = new BindableCollection<LabelViewModel>();
        }

        public CardViewModel InitializeWith(Card card)
        {
            var cover = card.Attachments.SingleOrDefault(x => x.Id == card.IdAttachmentCover);

            Id = card.Id;
            Name = card.Name;
            Desc = card.Desc;
            Due = card.Badges.Due;
            Votes = card.Badges.Votes;
            Comments = card.Badges.Comments;
            CheckItems = card.Badges.CheckItems;
            CheckItemsChecked = card.Badges.CheckItemsChecked;
            Attachments = card.Badges.Attachments;
            CoverUri = cover != null ? cover.Previews[0].Url : null;
            CoverHeight = cover != null ? cover.Previews[0].Height : 0;
            CoverWidth = cover != null ? cover.Previews[0].Width : 0;

            Members.Clear();
            Members.AddRange(card.Members.Select(x => new MemberViewModel(x)));

            Labels.Clear();
            Labels.AddRange(card.Labels.Select(x => new LabelViewModel(x)));

            return this;
        }
    }

    public class LabelViewModel
    {
        public string Color { get; set; }

        public string Name { get; set; }

        public LabelViewModel(Label lbl)
        {
            Color = lbl.Color;
            Name = lbl.Name;
        }
    }
}