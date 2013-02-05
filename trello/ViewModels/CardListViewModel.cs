using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Shell;
using Strilanc.Value;
using Telerik.Windows.Controls;
using trello.Assets;
using trellow.api.Data;
using trellow.api.Data.Services;
using trellow.api.Models;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public class CardListViewModel : PivotItemViewModel, IConfigureTheAppBar
    {
        private readonly INavigationService _navigationService;
        private readonly ICardService _cards;
        private readonly Func<CardViewModel> _cardFactory;
        private readonly Random _randomizer;

        public IObservableCollection<IGrouping<string, CardViewModel>> Cards { get; set; }

        public CardListViewModel(INavigationService navigationService,
                                 ICardService cards,
                                 Func<CardViewModel> cardFactory)
        {
            _navigationService = navigationService;
            _cards = cards;
            _cardFactory = cardFactory;

            DisplayName = "cards";

            Cards = new BindableCollection<IGrouping<string, CardViewModel>>();
            _randomizer = new Random();
        }

        protected override void OnViewLoaded(object view)
        {
            RefreshCards();
        }

        public void Open(ListBoxItemTapEventArgs args)
        {
            var context = args.Item.DataContext as CardViewModel;
            if (context == null)
                return;

            _navigationService.UriFor<CardDetailShellViewModel>()
                .WithParam(x => x.Id, context.Id)
                .Navigate();
        }

        private async void RefreshCards()
        {
            Cards.Clear();

            var cards = await _cards.Mine();
            cards.IfHasValueThenDo(x =>
            {
                Cards.Clear();

                var transformed = x
                    .Select(card => _cardFactory().InitializeWith(card))
                    .GroupBy(card => card.BoardName);

                Cards.AddRange(transformed);

                UpdateLiveTile(x);
            });
        }

        private void UpdateLiveTile(IReadOnlyCollection<Card> cards)
        {
            var tile = ShellTile.ActiveTiles.First();

            var data = new FlipTileData
            {
                Count = cards.Count,
                BackTitle = "",
                BackContent = "",
                WideBackContent = ""
            };

            if (cards.Any())
            {
                var index = _randomizer.Next(0, cards.Count);
                var first = cards.ElementAt(index);
                var name = first.Desc != null ? first.Name : "";
                var desc = first.Desc ?? "";

                data.BackTitle = name;
                data.BackContent = desc;
                data.WideBackContent = desc;
            }

            tile.Update(data);
        }

        public ApplicationBar ConfigureTheAppBar(ApplicationBar existing)
        {
            var refresh = new ApplicationBarIconButton(new AssetUri("Icons/dark/appbar.refresh.rest.png"))
            {Text = "refresh"};
            refresh.Click += (sender, args) => RefreshCards();
            existing.Buttons.Add(refresh);

            return existing;
        }
    }
}