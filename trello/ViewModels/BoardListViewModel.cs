using System;
using System.Linq;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Shell;
using TrelloNet;
using trello.Assets;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public class BoardListViewModel : PivotItemViewModel//, IConfigureTheAppBar
    {
        private readonly ITrello _api;
        private readonly Func<CardViewModel> _cardFactory;
        private string _name;
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

        public IObservableCollection<CardViewModel> Cards { get; set; }

        public BoardListViewModel(ITrello api, Func<CardViewModel> cardFactory)
        {
            _api = api;
            _cardFactory = cardFactory;

            Cards = new BindableCollection<CardViewModel>();
        }

        protected override void OnViewLoaded(object view)
        {
            RefreshLists();
        }

        private async void RefreshLists()
        {
            var cards = await _api.Cards.ForList(new ListId(Id));
            var vms = cards.Select(card => _cardFactory().InitializeWith(card));

            Cards.Clear();
            Cards.AddRange(vms);
        }

        public BoardListViewModel InitializeWith(List list)
        {
            Id = list.Id;
            Name = list.Name;

            return this;
        }

        public ApplicationBar Configure(ApplicationBar existing)
        {
            var addButton = new ApplicationBarIconButton(new AssetUri("Icons/dark/appbar.add.rest.png"))
            {
                Text = "add card"
            };
            addButton.Click += (sender, args) => AddCard();
            existing.Buttons.Add(addButton);

            return existing;
        }

        private static void AddCard()
        {
        }

        private void RemoveCard()
        {
        }
    }
}