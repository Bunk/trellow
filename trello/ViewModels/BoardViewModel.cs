using System;
using System.Linq;
using Caliburn.Micro;
using trellow.api;
using trellow.api.Data;
using trellow.api.Models;

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

        public void InitializeWith(Board board)
        {
            Id = board.Id;
            Name = board.Name;
            Desc = board.Desc;
            IsPrivate = board.Prefs.PermissionLevel == "private";

            var lists = board.Lists.Select(BuildListViewModel);
            Items.Clear();
            Items.AddRange(lists);
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