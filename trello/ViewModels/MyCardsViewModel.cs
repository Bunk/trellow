using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Shell;
using TrelloNet;
using trello.Assets;
using trello.Services;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public sealed class MyCardsViewModel : PivotItemViewModel, IConfigureTheAppBar
    {
        private readonly ITrello _api;
        private readonly IProgressService _progress;
        private readonly Func<CardViewModel> _cardFactory;
        private readonly Random _randomizer;

        public IObservableCollection<IGrouping<string, CardViewModel>> Cards { get; set; }

        public MyCardsViewModel(ITrello api,
                                IProgressService progress,
                                Func<CardViewModel> cardFactory)
        {
            _api = api;
            _progress = progress;
            _cardFactory = cardFactory;

            DisplayName = "cards";

            Cards = new BindableCollection<IGrouping<string, CardViewModel>>();
            _randomizer = new Random();
        }

        protected override void OnInitialize()
        {
            RefreshCards();
        }

        private async void RefreshCards()
        {
            _progress.Show("Refreshing");

            try
            {
                var cards = (await _api.Async.Cards.ForMe());
                var boards = (await _api.Async.Boards.ForMe());

                var vms = cards
                    .Select(card =>
                    {
                        var vm = _cardFactory().InitializeWith(card);

                        var board = boards.FirstOrDefault(x => x.Id == card.IdBoard);
                        if (board != null)
                            vm.BoardName = board.Name;

                        return vm;
                    })
                    .GroupBy(card => card.BoardName);

                Cards.Clear();
                Cards.AddRange(vms);

                UpdateLiveTile(cards.ToList());
            }
            catch (Exception)
            {
                MessageBox.Show("Your cards could not be loaded.  Please " +
                                "ensure that you have an active internet connection.");
            }

            _progress.Hide();
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

        public ApplicationBar Configure(ApplicationBar existing)
        {
            var refresh = new ApplicationBarIconButton(new AssetUri("Icons/dark/appbar.refresh.rest.png"))
            {Text = "refresh"};
            refresh.Click += (sender, args) => RefreshCards();
            existing.Buttons.Add(refresh);

            return existing;
        }
    }
}