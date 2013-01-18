using System;
using System.Linq;
using Caliburn.Micro;
using trellow.api.Models;

namespace trello.ViewModels
{
    public class CardDetailOverviewViewModel : PivotItemViewModel
    {
        private string _boardName;

        public string Id { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public int CheckItems { get; set; }

        public int CheckItemsChecked { get; set; }

        public int Checklists { get; set; }

        public int Members { get; set; }

        public int Attachments { get; set; }

        public int Votes { get; set; }

        public DateTime? DueDate { get; set; }

        public IObservableCollection<LabelViewModel> Labels { get; set; }

        public CardDetailOverviewViewModel()
        {
            DisplayName = "overview";
        }

        public CardDetailOverviewViewModel InitializeWith(Card card)
        {
            Id = card.Id;
            Name = card.Name;
            Desc = card.Desc;

            Checklists = card.IdChecklists.Count;
            CheckItems = card.Badges.CheckItems;
            CheckItemsChecked = card.Badges.CheckItemsChecked;
            Members = card.Members.Count;
            Attachments = card.Badges.Attachments;
            Votes = card.Badges.Votes;
            DueDate = card.Badges.Due;

            Labels = new BindableCollection<LabelViewModel>(card.Labels.Select(x => new LabelViewModel(x)));

            return this;
        }

        protected override void OnInitialize()
        {
        }
    }
}