using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Shell;
using Strilanc.Value;
using Trellow.UI;
using Trellow.ViewModels.Boards;
using Trellow.ViewModels.Cards;
using Trellow.ViewModels.Notifications;
using trellow.api;
using trellow.api.Internal;
using trellow.api.Notifications;

namespace Trellow.ViewModels
{
    public sealed class MyNotificationsViewModel : PivotItemViewModel<MyNotificationsViewModel>
    {
        private readonly INavigationService _navigation;
        private readonly ITrello _api;

        [UsedImplicitly]
        public IObservableCollection<NotificationViewModel> Notifications { get; set; }

        public MyNotificationsViewModel(INavigationService navigation, ITrello api)
        {
            _navigation = navigation;
            _api = api;

            DisplayName = "notifications";
            Notifications = new BindableCollection<NotificationViewModel>();
        }

        protected override void OnInitialize()
        {
            Load();
        }

        protected override void OnActivate()
        {
            ApplicationBar.UpdateWith(config =>
            {
                config.Setup(bar => bar.AddButton("refresh", new AssetUri("Icons/dark/appbar.refresh.rest.png"), Load));
                config.Defaults();
            });
        }

        [UsedImplicitly]
        public void OpenCard(string cardId)
        {
            _navigation.UriFor<CardDetailPivotViewModel>()
                       .WithParam(vm => vm.Id, cardId)
                       .Navigate();
        }

        [UsedImplicitly]
        public void OpenBoard(string boardId)
        {
            _navigation.UriFor<BoardViewModel>()
                       .WithParam(vm => vm.Id, boardId)
                       .Navigate();
        }

        private async void Load()
        {
            var types = new List<NotificationType>
            {
                NotificationType.AddedToBoard,
                NotificationType.AddedToCard,
                NotificationType.ChangeCard,
                NotificationType.CloseBoard,
                NotificationType.CommentCard,
                NotificationType.MentionedOnCard
            };
            var notifications = (await _api.Notifications.ForMe(types, ReadFilter.Unread, new Paging(15, 0))).ToList();
            var vms = notifications.Select(NotificationViewModel.Create).WhereHasValue();

            Notifications.Clear();
            Notifications.AddRange(vms);

            UpdateLiveTile(notifications);
        }

        private static void UpdateLiveTile(ICollection<Notification> notifications)
        {
            var tile = ShellTile.ActiveTiles.First();

            var data = new StandardTileData
            {
                Count = notifications.Count,
                BackTitle = "",
                BackContent = ""
            };
            tile.Update(data);
        }
    }
}