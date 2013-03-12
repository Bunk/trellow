using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using TrelloNet;
using trellow.api;

namespace trello.ViewModels
{
    public class BoardViewModel : PivotViewModel
    {
        private readonly ITrello _api;
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
                              ITrello api,
                              INavigationService navigation,
                              Func<BoardListViewModel> listFactory) : base(settings, navigation)
        {
            _api = api;
            _listFactory = listFactory;
        }

        protected override async void OnInitialize()
        {
            var board = await _api.Boards.WithId(Id);
            if (board == null)
                return;

            var lists = await _api.Lists.ForBoard(board);

            InitializeBoard(board);
            InitializeLists(lists);

            ActivateItem(Items[0]);
        }

        public BoardViewModel InitializeLists(IEnumerable<List> lists)
        {
            var vms = lists.Select(list => _listFactory().InitializeWith(list));
            Items.Clear();
            Items.AddRange(vms);

            return this;
        }

        public BoardViewModel InitializeBoard(Board board)
        {
            Id = board.Id;
            Name = board.Name;
            Desc = board.Desc;
            IsPrivate = board.Prefs.PermissionLevel == PermissionLevel.Private;

            return this;
        }
    }
}