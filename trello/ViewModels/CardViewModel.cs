using System;
using System.Linq;
using Caliburn.Micro;
using trello.Services.Models;

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

        public bool HasDescription { get; set; }

        public IObservableCollection<MemberViewModel> Members { get; set; }

        public IObservableCollection<LabelViewModel> Labels { get; set; }

        public CardViewModel(Card card)
        {
            Id = card.Id;
            Name = card.Name;
            Desc = card.Desc;
            Due = card.Badges.Due;
            Votes = card.Badges.Votes;
            Comments = card.Badges.Comments;
            CheckItems = card.Badges.CheckItems;
            CheckItemsChecked = card.Badges.CheckItemsChecked;
            Attachments = card.Badges.Attachments;
            HasDescription = card.Badges.Description;
            Members = new BindableCollection<MemberViewModel>(card.Members.Select(m => new MemberViewModel(m)));
            Labels = new BindableCollection<LabelViewModel>(card.Labels.Select(l => new LabelViewModel(l)));
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