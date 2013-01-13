using System.Linq;
using Caliburn.Micro;
using trello.Services.Data;

namespace trello.ViewModels
{
    public class BoardListViewModel : PivotItemViewModel
    {
        private readonly ICardService _cardService;
        private string _name;
        private bool _subscribed;
        private string _id;

        public string Id
        {
            get { return _id; }
            set
            {
                if (value == _id) return;
                _id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                NotifyOfPropertyChange(() => Name);

                DisplayName = _name;
            }
        }

        public bool Subscribed
        {
            get { return _subscribed; }
            set
            {
                if (value.Equals(_subscribed)) return;
                _subscribed = value;
                NotifyOfPropertyChange(() => Subscribed);
            }
        }

        public IObservableCollection<CardViewModel> Cards { get; set; }

        public BoardListViewModel(ICardService cardService)
        {
            _cardService = cardService;
            Cards = new BindableCollection<CardViewModel>();
        }

        protected override async void OnInitialize()
        {
            Cards.Clear();

            var cards = await _cardService.InList(Id);
            Cards.AddRange(cards.Select(c => new CardViewModel(c)));
        }
    }
}