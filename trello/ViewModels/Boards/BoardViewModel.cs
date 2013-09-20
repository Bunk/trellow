using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using JetBrains.Annotations;
using trello.Services;
using trellow.api;
using trellow.api.Boards;
using trellow.api.Lists;

namespace trello.ViewModels.Boards
{
    [UsedImplicitly]
    public class BoardViewModel : PivotViewModel
    {
        private readonly ITrello _api;
        private readonly Func<BoardListViewModel> _listFactory;
        private string _name;
        private string _id;
        private string _desc;
        private bool _isPrivate;

        [UsedImplicitly]
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

        [UsedImplicitly]
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

        [UsedImplicitly]
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

        [UsedImplicitly]
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

        [UsedImplicitly]
        public string SelectedListId { get; set; }

        public BoardViewModel(ITrelloApiSettings settings,
                              ITrello api,
                              INavigationService navigation,
                              IApplicationBar applicationBar,
                              Func<BoardListViewModel> listFactory) : base(navigation, applicationBar)
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
            InitializeLists(lists.ToList());

            var selectedItem = FindSelectedItem(Items, SelectedListId);

            ActivateItem(selectedItem);
        }

        private static IScreen FindSelectedItem(IList<IScreen> items, string selectedListId)
        {
            var selectedIndex = 0;
            if (!string.IsNullOrEmpty(selectedListId))
            {
                var vms = items.OfType<BoardListViewModel>().ToList();
                selectedIndex = vms.FindIndex(vm => vm.Id == selectedListId);
            }
            return items[selectedIndex];
        }

        private void InitializeLists(List<List> lists)
        {
            var vms = lists.Select(list => _listFactory()
                                               .InitializeWith(lists, list)
                                               .Bind(AppBar));

            Items.Clear();
            Items.AddRange(vms);
        }

        private void InitializeBoard(Board board)
        {
            Id = board.Id;
            Name = board.Name;
            Desc = board.Desc;
            IsPrivate = board.Prefs.PermissionLevel == PermissionLevel.Private;
        }
    }
}