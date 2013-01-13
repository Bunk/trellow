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
    public class BoardsViewModel : PivotItemViewModel, IConfigureTheAppBar
    {
        private readonly INavigationService _navigationService;
        private readonly IBoardService _boardService;
        private readonly Func<BoardViewModel> _boardFactory;

        public IObservableCollection<BoardViewModel> Boards { get; private set; }

        public BoardsViewModel(INavigationService navigationService, IBoardService boardService, Func<BoardViewModel> boardFactory)
        {
            _navigationService = navigationService;
            _boardService = boardService;
            _boardFactory = boardFactory;

            DisplayName = "boards";

            Boards = new BindableCollection<BoardViewModel>();
        }

        public void OpenBoard(BoardViewModel context)
        {
            var id = context.Id;
            _navigationService.UriFor<BoardViewModel>().WithParam(vm => vm.Id, id).Navigate();
            //_navigationService.Navigate(new Uri("/Views/Boards/BoardView.xaml", UriKind.Relative));
        }

        protected override void OnViewLoaded(object view)
        {
            RefreshBoards();
        }

        private async void RefreshBoards()
        {
            Boards.Clear();

            var boards = await _boardService.Mine();

            Boards.AddRange(boards.Select(BuildBoard));
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