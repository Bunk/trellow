using System;
using System.Reflection;
using TrelloNet;
using JetBrains.Annotations;
using Strilanc.Value;

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

        public static May<NotificationViewModel> Create(Notification dto)
        {
            // Factory method... use convention '{NotifictionName}' + 'ViewModel'
            var dtoType = dto.GetType();
            var vmName = dtoType.Name + "ViewModel";
            var type = Assembly.GetExecutingAssembly().GetType(vmName, false);
            if (type == null)
                return May<NotificationViewModel>.NoValue;

            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
                return May<NotificationViewModel>.NoValue;

            var instance = ctor.Invoke(null) as NotificationViewModel;
            if (instance == null)
                return May<NotificationViewModel>.NoValue;

            instance.Init(dto);

            var initMethod = type.GetMethod("Init", new[] {dtoType});
            if (initMethod != null)
                initMethod.Invoke(instance, new object[] {dto});

            return instance;
        }

        protected virtual NotificationViewModel Init(Notification dto)
        {
            Id = dto.Id;
            IdMemberCreator = dto.IdMemberCreator;
            Date = dto.Date;
            Unread = dto.Unread;

            return this;
        }
    }
}