using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using trello.Services.Models;

namespace trello.ViewModels
{
    public class CardDetailOverviewViewModel : PivotItemViewModel
    {
        private string _boardName;

        public string Id { get; set; }
        public string BoardName
        {
            get { return _boardName; }
            set
            {
                if (value == _boardName) return;
                _boardName = value;
                NotifyOfPropertyChange(() => BoardName);
            }
        }

        public CardDetailOverviewViewModel()
        {
            DisplayName = "overview";
        }

        public CardDetailOverviewViewModel InitializeWith(Card card)
        {
            Id = card.Id;
            BoardName = card.Board.Name;
            return this;
        }

        protected override void OnInitialize()
        {

        }
    }
}
