using System;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Strilanc.Value;
using TrelloNet;
using trellow.api;
using trellow.api.Data;
using trellow.api.Data.Services;
using Board = trellow.api.Models.Board;
using List = trellow.api.Models.List;

namespace trello.ViewModels
{
    public class BoardViewModel : PivotViewModel
    {
        private readonly IBoardService _boardService;
        private readonly Func<BoardListViewModel> _listFactory;
        private string _name;
        private string _id;
        private string _desc;
        private bool _isPrivate;

        public string Id
        {
            get { return _id; }
            set
            {
                if (value == _id) return;
                _id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public string Desc
        {
            get { return _desc; }
            set
            {
                if (value == _desc) return;
                _desc = value;
                NotifyOfPropertyChange(() => Desc);
            }
        }

        public bool IsPrivate
        {
            get { return _isPrivate; }
            set
            {
                if (value.Equals(_isPrivate)) return;
                _isPrivate = value;
                NotifyOfPropertyChange(() => IsPrivate);
            }
        }

        public BoardViewModel(ITrelloApiSettings settings,
                              INavigationService navigation,
                              IBoardService boardService,
                              Func<BoardListViewModel> listFactory) : base(settings, navigation)
        {
            _boardService = boardService;
            _listFactory = listFactory;
        }

        protected override async void OnInitialize()
        {
            base.OnInitialize();

            var board = await _boardService.WithId(Id);

            InitializeWith(board);

            ActivateItem(Items[0]);
        }

        public void InitializeWith(May<Board> board)
        {
            board.IfHasValueThenDo(x =>
            {
                Id = x.Id;
                Name = x.Name;
                Desc = x.Desc;
                IsPrivate = x.Prefs.PermissionLevel == "private";

                var lists = x.Lists.Select(BuildListViewModel);
                Items.Clear();
                Items.AddRange(lists);
            }).ElseDo(() => MessageBox.Show("The board could not be loaded.  " +
                                            "Please ensure that you have an active internet connection."));
        }

        public void InitializeWith(TrelloNet.Board board)
        {
            Id = board.Id;
            Name = board.Name;
            Desc = board.Desc;
            IsPrivate = board.Prefs.PermissionLevel == PermissionLevel.Private;
        }

        private BoardListViewModel BuildListViewModel(List list)
        {
            var vm = _listFactory();
            vm.Id = list.Id;
            vm.Name = list.Name;
            vm.Subscribed = list.Subscribed;
            return vm;
        }
    }
}