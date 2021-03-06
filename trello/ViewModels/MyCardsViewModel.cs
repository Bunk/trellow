﻿using System;
using System.Linq;
using Caliburn.Micro;
using JetBrains.Annotations;
using trello.Assets;
using trello.Extensions;
using trello.ViewModels.Cards;
using trellow.api;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public sealed class MyCardsViewModel : PivotItemViewModel<MyCardsViewModel>
    {
        private readonly ITrello _api;
        private readonly Func<CardViewModel> _cardFactory;

        [UsedImplicitly]
        public IObservableCollection<IGrouping<string, CardViewModel>> Cards { get; set; }

        public MyCardsViewModel(ITrello api, Func<CardViewModel> cardFactory)
        {
            _api = api;
            _cardFactory = cardFactory;

            DisplayName = "cards";

            Cards = new BindableCollection<IGrouping<string, CardViewModel>>();
        }

        protected override void OnInitialize()
        {
            RefreshCards();
        }

        protected override void OnActivate()
        {
            ApplicationBar.UpdateWith(config =>
            {
                config.Setup(bar => bar.AddButton("refresh", new AssetUri("Icons/dark/appbar.refresh.rest.png"), RefreshCards));
                config.Defaults();
            });
        }

        private async void RefreshCards()
        {
            var cards = (await _api.Cards.ForMe()).ToList();
            if (cards.Count == 0)
            {
                Cards.Clear();
                return;
            }

            var boards = (await _api.Boards.ForMe()).ToList();
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
        }
    }
}