using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Shell;
using Strilanc.Value;
using Telerik.Windows.Controls;
using TrelloNet;
using trello.Assets;
using trellow.api.Data.Services;
using Board = trellow.api.Models.Board;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public class BoardsViewModel : PivotItemViewModel, IConfigureTheAppBar
    {
        private readonly Func<BoardViewModel> _boardFactory;
        private readonly IBoardService _boardService;
        private readonly ITrello _api;
        private readonly TrelloCoordinator _data;
        private readonly INavigationService _navigationService;

        public BoardsViewModel(INavigationService navigationService,
                               IBoardService boardService,
            ITrello api,
            TrelloCoordinator data,
                               Func<BoardViewModel> boardFactory)
        {
            _navigationService = navigationService;
            _boardService = boardService;
            _api = api;
            _data = data;
            _boardFactory = boardFactory;

            DisplayName = "boards";

            Boards = new BindableCollection<BoardViewModel>();
        }

        public IObservableCollection<BoardViewModel> Boards { get; private set; }

        public ApplicationBar ConfigureTheAppBar(ApplicationBar existing)
        {
            var refresh = new ApplicationBarIconButton(new AssetUri("Icons/dark/appbar.refresh.rest.png"))
            {Text = "refresh"};
            refresh.Click += (sender, args) => RefreshBoards();
            existing.Buttons.Add(refresh);

            return existing;
        }

        public void Open(ListBoxItemTapEventArgs args)
        {
            var context = args.Item.DataContext as BoardViewModel;
            if (context == null)
                return;

            _navigationService.UriFor<BoardViewModel>()
                              .WithParam(x => x.Id, context.Id)
                              .Navigate();
        }

        protected override void OnInitialize()
        {
            RefreshBoards();
        }

        private async void RefreshBoards()
        {
            var boards = await _data.Execute(() => _api.Async.Boards.ForMe(), "/boards/mine");
            boards
                .IfHasValueThenDo(x =>
                {
                    Boards.Clear();
                    Boards.AddRange(x.Select(BuildBoard));
                })
                .ElseDo(() => MessageBox.Show("Your boards could not be loaded at this time.  " +
                                              "Please ensure that you have an internet connection."));
        }

        private BoardViewModel BuildBoard(TrelloNet.Board board)
        {
            var vm = _boardFactory();
            vm.InitializeWith(board);
            return vm;
        }
    }
}