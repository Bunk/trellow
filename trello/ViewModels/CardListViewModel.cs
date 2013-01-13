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

    public class MemberViewModel : Screen
    {
        public string ImageUri { get; set; }

        public MemberViewModel(Member member)
        {
            ImageUri = string.Format("https://trello-avatars.s3.amazonaws.com/{0}/30.png", member.AvatarHash);
        }
    }
}
