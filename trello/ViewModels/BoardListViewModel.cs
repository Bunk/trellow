using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using trello.Services;
using trello.Services.Models;

namespace trello.ViewModels
{
    public class BoardListViewModel : ViewModelBase
    {
        private readonly IProgressService _progressService;
        private readonly IBoardService _boardService;

        public IObservableCollection<BoardViewModel> Boards { get; private set; }

        public BoardListViewModel(IProgressService progressService, IBoardService boardService)
        {
            _progressService = progressService;
            _boardService = boardService;

            DisplayName = "boards";
            Boards = new BindableCollection<BoardViewModel>();
        }

        protected override void OnViewLoaded(object view)
        {
            UpdateMyBoards();

            //MessageBox.Show("The boards would be loaded here.");

            base.OnViewLoaded(view);
        }

        public void UpdateMyBoards()
        {
            Boards.Clear();

            _progressService.Show();

            //var boards = await _boardService.Mine();
            var boards = new List<Board>
            {
                new Board { Name = "Work @ Home", Desc = "This is a simple description for this board.", Prefs = new Preferences { PermissionLevel = "private" }},
                new Board { Name = "Trellow", Desc = "This is a short description.", Prefs = new Preferences { PermissionLevel = "private" }},
                new Board { Name = "Cardboard", Desc = "This should be a really, really, really, really long description for this board.", Prefs = new Preferences { PermissionLevel = "public"}}
            };
            Boards.AddRange(boards.Select(b => new BoardViewModel(b)));

            _progressService.Hide();
        }
    }

    public class BoardViewModel : ViewModelBase
    {
        public string Name { get; set; }

        public string Desc { get; set; }

        public bool IsPrivate { get; set; }

        public BoardViewModel(Board board)
        {
            Name = board.Name;
            Desc = board.Desc;
            IsPrivate = board.Prefs.PermissionLevel == "private";
        }
    }
}
