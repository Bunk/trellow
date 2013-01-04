using System;
using System.Linq;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Shell;
using trello.Assets;
using trello.Services.Data;
using trello.Services.Models;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public class CardListViewModel : PivotItemViewModel, IConfigureTheAppBar
    {
        private readonly ICardService _cards;

        public IObservableCollection<CardViewModel> Cards { get; set; }

        public CardListViewModel(ICardService cards)
        {
            _cards = cards;

            DisplayName = "cards";

            Cards = new BindableCollection<CardViewModel>();
        }

        protected override void OnViewLoaded(object view)
        {
            RefreshCards();
        }

        private async void RefreshCards()
        {
            Cards.Clear();

            var cards = await _cards.Mine();

            Cards.AddRange(cards.Select(c => new CardViewModel(c)));
        }

        public ApplicationBar ConfigureTheAppBar(ApplicationBar existing)
        {
            var refresh = new ApplicationBarIconButton(new AssetUri("Icons/dark/appbar.refresh.rest.png")) { Text = "refresh" };
            refresh.Click += (sender, args) => RefreshCards();
            existing.Buttons.Add(refresh);

            return existing;
        }
    }

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

    public class MemberViewModel : Screen
    {
        public string ImageUri { get; set; }

        public MemberViewModel(Member member)
        {
            ImageUri = string.Format("https://trello-avatars.s3.amazonaws.com/{0}/30.png", member.AvatarHash);
        }
    }
}
