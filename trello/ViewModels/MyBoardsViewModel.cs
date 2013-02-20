using System;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;
using TrelloNet;
using trello.Assets;
using trello.Services;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public sealed class MyBoardsViewModel : PivotItemViewModel, IConfigureTheAppBar
    {
        private readonly Func<BoardViewModel> _boardFactory;
        private readonly ITrello _api;
        private readonly INavigationService _navigationService;
        private readonly IProgressService _progress;

        public MyBoardsViewModel(INavigationService navigationService,
                                 IProgressService progress,
                                 ITrello api,
                                 Func<BoardViewModel> boardFactory)
        {
            _navigationService = navigationService;
            _progress = progress;
            _api = api;
            _boardFactory = boardFactory;

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
            _progress.Show("Refreshing...");

            try
            {
                var boards = (await _api.Async.Boards.ForMe());

                Boards.Clear();
                Boards.AddRange(boards.Select(BuildBoard));
            }
            catch (Exception)
            {
                MessageBox.Show("Your boards could not be loaded at this time.  " +
                                "Please ensure that you have an internet connection.");
            }
            _progress.Hide();
        }

        private BoardViewModel BuildBoard(Board board)
        {
            return _boardFactory().InitializeBoard(board);
        }
    }
}