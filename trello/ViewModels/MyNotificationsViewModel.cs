using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Shell;
using TrelloNet;
using trello.Assets;
using trello.ViewModels.Notifications;
using Strilanc.Value;

namespace trello.ViewModels
{
    public sealed class MyNotificationsViewModel : PivotItemViewModel, IConfigureTheAppBar
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
            RefreshNotifications();
        }

        public ApplicationBar Configure(ApplicationBar existing)
        {
            var refresh = new ApplicationBarIconButton(new AssetUri("Icons/dark/appbar.refresh.rest.png")) { Text = "refresh" };
            refresh.Click += (sender, args) => RefreshNotifications();
            existing.Buttons.Add(refresh);

            return existing;
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

        private async void RefreshNotifications()
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
            var notifications = await _api.Notifications.ForMe(types, paging: new Paging(15, 0));
            var vms = notifications.Select(NotificationViewModel.Create).WhereHasValue().ToList();

            Notifications.Clear();
            Notifications.AddRange(vms);
        }
    }
}
