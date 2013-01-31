using System;
using System.Linq;
using System.Windows;
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
    public class BoardsViewModel : PivotItemViewModel, IConfigureTheAppBar
    {
        private readonly INavigationService _navigationService;
        private readonly IBoardService _boardService;
        private readonly Func<BoardViewModel> _boardFactory;

        public IObservableCollection<BoardViewModel> Boards { get; private set; }

        public BoardsViewModel(INavigationService navigationService, IBoardService boardService,
                               Func<BoardViewModel> boardFactory)
        {
            _navigationService = navigationService;
            _boardService = boardService;
            _boardFactory = boardFactory;

            DisplayName = "boards";

            Boards = new BindableCollection<BoardViewModel>();
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

        protected override void OnViewLoaded(object view)
        {
            RefreshBoards();
        }

        private async void RefreshBoards()
        {
            Boards.Clear();

            var boards = await _boardService.Mine();
            boards
                .IfHasValueThenDo(x => Boards.AddRange(x.Select(BuildBoard)))
                .ElseDo(() => MessageBox.Show("Your boards could not be loaded at this time.  " +
                                              "Please ensure a valid internet connection."));
        }

        private BoardViewModel BuildBoard(Board board)
        {
            var vm = _boardFactory();
            vm.InitializeWith(board);
            return vm;
        }

        public ApplicationBar ConfigureTheAppBar(ApplicationBar existing)
        {
            var refresh = new ApplicationBarIconButton(new AssetUri("Icons/dark/appbar.refresh.rest.png"))
            {Text = "refresh"};
            refresh.Click += (sender, args) => RefreshBoards();
            existing.Buttons.Add(refresh);

            return existing;
        }
    }
}