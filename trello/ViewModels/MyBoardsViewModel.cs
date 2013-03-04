using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;
using TrelloNet;
using trello.Assets;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public sealed class MyBoardsViewModel : PivotItemViewModel, IConfigureTheAppBar
    {
        private readonly ITrello _api;
        private readonly INavigationService _navigationService;

        public MyBoardsViewModel(INavigationService navigationService, ITrello api)
        {
            _navigationService = navigationService;
            _api = api;

            DisplayName = "boards";

            Boards = new BindableCollection<BoardViewModel>();
        }

        public IObservableCollection<BoardViewModel> Boards { get; private set; }

        public ApplicationBar Configure(ApplicationBar existing)
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

            _navigationService.UriFor<ViewModels.BoardViewModel>()
                .WithParam(x => x.Id, context.Id)
                .Navigate();
        }

        protected override void OnInitialize()
        {
            RefreshBoards();
        }

        private async void RefreshBoards()
        {
            var boards = (await _api.Boards.ForMe());

            Boards.Clear();
            Boards.AddRange(boards.Select(BuildBoard));
        }

        private BoardViewModel BuildBoard(Board board)
        {
            return new BoardViewModel().Initialize(board);
        }

        public class BoardViewModel : PropertyChangedBase
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public string Desc { get; set; }

            public string Organization { get; set; }

            public string Prefs { get; set; }

            public bool IsPrivate { get; set; }

            public bool IsOrganization { get; set; }

            public bool IsPinned { get; set; }

            public BoardViewModel Initialize(Board board)
            {
                Id = board.Id;
                Name = board.Name;
                Desc = board.Desc;
                IsPrivate = board.Prefs.PermissionLevel == PermissionLevel.Private;
                IsOrganization = board.Prefs.PermissionLevel == PermissionLevel.Org;
                IsPinned = board.Pinned;

                var prefs = new List<string>();
                if (board.Prefs.PermissionLevel == PermissionLevel.Private)
                    prefs.Add("private");
                else if (board.Prefs.PermissionLevel == PermissionLevel.Org)
                    prefs.Add("organization");
                else if (board.Prefs.PermissionLevel == PermissionLevel.Public)
                    prefs.Add("public");

                if (board.Prefs.Comments == CommentPermission.Members)
                    prefs.Add("member comments");
                else if (board.Prefs.Comments == CommentPermission.Disabled)
                    prefs.Add("comments disabled");
                else if (board.Prefs.Comments == CommentPermission.Org)
                    prefs.Add("org comments");
                else if (board.Prefs.Comments == CommentPermission.Public)
                    prefs.Add("public comments");

                Prefs = string.Join(", ", prefs);

                return this;
            }
        }
    }
}