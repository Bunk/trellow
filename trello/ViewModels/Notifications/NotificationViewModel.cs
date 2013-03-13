using System;
using System.Reflection;
using Caliburn.Micro;
using TrelloNet;
using JetBrains.Annotations;
using Strilanc.Value;
using trello.Extensions;

namespace trello.ViewModels.Notifications
{
    public abstract class NotificationViewModel
    {
        [UsedImplicitly]
        public string Id { get; set; }

        [UsedImplicitly]
        public bool Unread { get; set; }

        [UsedImplicitly]
        public DateTime Date { get; set; }

        [UsedImplicitly]
        public string IdMemberCreator { get; set; }

        [UsedImplicitly]
        public string MemberCreator { get; set; }

        [UsedImplicitly]
        public string MemberCreatorAvatarUrl { get; set; }

        public static May<NotificationViewModel> Create(Notification dto)
        {
            // Factory method... use convention '{NotifictionName}' + 'ViewModel'
            var dtoType = dto.GetType();
            var vmName = "trello.ViewModels.Notifications." + dtoType.Name + "ViewModel";
            var type = Assembly.GetExecutingAssembly().GetType(vmName, false);
            if (type == null || type == typeof(NotificationViewModel)) // abstract type
                return May<NotificationViewModel>.NoValue;

            // Try to use the container
            var instance = IoC.GetInstance(type, null) as NotificationViewModel;
            if (instance == null)
                return May<NotificationViewModel>.NoValue;

            return instance.Init(dto);
        }

        protected virtual NotificationViewModel Init(Notification dto)
        {
            Id = dto.Id;
            IdMemberCreator = dto.IdMemberCreator;
            MemberCreator = dto.MemberCreator.FullName;
            MemberCreatorAvatarUrl = dto.MemberCreator.AvatarHash.ToAvatarUrl();
            Date = dto.Date;
            Unread = dto.Unread;

            return this;
        }
    }
}