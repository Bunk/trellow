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
    public class BoardListViewModel : PivotItemViewModel, IConfigureTheAppBar
    {
        private readonly IBoardService _boardService;

        public IObservableCollection<BoardViewModel> Boards { get; private set; }

        public BoardListViewModel(IBoardService boardService)
        {
            _boardService = boardService;

            DisplayName = "boards";

            Boards = new BindableCollection<BoardViewModel>();
        }

        protected override void OnViewLoaded(object view)
        {
            RefreshBoards();
        }

        private async void RefreshBoards()
        {
            Boards.Clear();

            var boards = await _boardService.Mine();

            Boards.AddRange(boards.Select(b => new BoardViewModel(b)));
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

    public class BoardViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public bool IsPrivate { get; set; }

        public BoardViewModel(Board board)
        {
            Id = board.Id;
            Name = board.Name;
            Desc = board.Desc;
            IsPrivate = board.Prefs.PermissionLevel == "private";
        }
    }
}