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

        public string Due { get; set; }

        public int Votes { get; set; }

        public int CheckItems { get; set; }

        public int CheckItemsChecked { get; set; }

        public IObservableCollection<MemberViewModel> Members { get; set; } 

        public CardViewModel(Card card)
        {
            Id = card.Id;
            Name = card.Name;
            Desc = card.Desc;
            Due = card.Due.HasValue ? card.Due.Value.ToString("d MMM") : null;
            Votes = card.Badges.Votes;
            CheckItems = card.Badges.CheckItems;
            CheckItemsChecked = card.Badges.CheckItemsChecked;
            Members = new BindableCollection<MemberViewModel>(card.Members.Select(m => new MemberViewModel(m)));
        }
    }
}