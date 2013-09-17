﻿using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using trello.Extensions;
using trello.Services;
using trello.Services.Cache;
using trello.Services.Handlers;
using trello.ViewModels;
using trello.ViewModels.Boards;
using trello.ViewModels.Checklists;
using trello.ViewModels.Notifications;
using trellow.api;
using trellow.api.Data;
using trellow.api.Internal;

namespace trello
{
    public class ApplicationBootstrapper : PhoneBootstrapper
    {
        private PhoneContainer _container;
        private LocalyticsSession _localytics;

        protected override void Configure()
        {
            _container = new PhoneContainer();
            _container.RegisterPhoneServices(RootFrame);

            _container.Instance(RootFrame);

            _container.Singleton<ICache, FileSystemCache>();
            
            // View Models
            _container.Singleton<SplashViewModel>();
            _container.Singleton<AboutViewModel>();
            _container.Singleton<ShellViewModel>();
            _container.Singleton<MyBoardsViewModel>();
            _container.Singleton<MyCardsViewModel>();
            _container.Singleton<MyNotificationsViewModel>();
            _container.Singleton<ProfileViewModel>();

            _container.PerRequest<BoardViewModel>();
            _container.PerRequest<BoardListViewModel>();
            _container.PerRequest<CardViewModel>();
            _container.PerRequest<CardDetailPivotViewModel>();
            _container.PerRequest<CardDetailOverviewViewModel>();
            _container.PerRequest<CardDetailChecklistViewModel>();
            _container.PerRequest<CardDetailAttachmentsViewModel>();
            _container.PerRequest<CardDetailMembersViewModel>();
            _container.PerRequest<ChecklistViewModel>();
            _container.PerRequest<ChecklistItemViewModel>();
            _container.PerRequest<AttachmentViewModel>();
            _container.AllTransientTypesOf<NotificationViewModel>();

            // Event handlers
            _container.AllSingletonTypesOf<AbstractHandler>();

            // Request handling
            _container.Singleton<INetworkService, NetworkService>();
            _container.Singleton<IProgressService, ProgressService>();
            _container.Singleton<ITrelloApiSettings, TrelloSettings>();
            
#if DISCONNECTED
            _container.Singleton<IRequestClient, JsonFileRestClient>();
#else
            _container.Singleton<IRequestClient, TrelloRestClient>();
#endif
            var network = _container.Get<INetworkService>();
            var client = AugmentClient(_container);
            var trello = new Trello(network, client);
            _container.Instance<ITrello>(trello);

            PhoneToolkitConventions.Install();
            TelerikConventions.Install();

            // Force creation
            _container.InstantiateInstancesOf<AbstractHandler>();
        }

        private static IRequestClient AugmentClient(SimpleContainer container)
        {
            var progress = container.Get<IProgressService>();
            var handler = container.Get<IRequestClient>();

            return new ErrorHandlingRestClient(new ProgressAwareRestClient(handler, progress));
        }

        protected override PhoneApplicationFrame CreatePhoneApplicationFrame()
        {
            var frame = new TransitionFrame();

            TiltEffect.SetIsTiltEnabled(frame, true);

            return frame;
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void OnLaunch(object sender, LaunchingEventArgs e)
        {
            base.OnLaunch(sender, e);

            CreateAndStartAnalyticsSession();
        }

        protected override void OnClose(object sender, ClosingEventArgs e)
        {
            _localytics.close();

            base.OnClose(sender, e);
        }

        protected override void OnActivate(object sender, ActivatedEventArgs e)
        {
            base.OnActivate(sender, e);

            CreateAndStartAnalyticsSession();
        }

        protected override void OnDeactivate(object sender, DeactivatedEventArgs e)
        {
            _localytics.close();

            base.OnDeactivate(sender, e);
        }

        protected override void OnUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            var attributes = new Dictionary<string, string>
            {
                {"exception", e.ExceptionObject.Message},
                {"stack", e.ExceptionObject.StackTrace}
            };
            _localytics.tagEvent("App Crash", attributes);

            base.OnUnhandledException(sender, e);
        }

        private void CreateAndStartAnalyticsSession()
        {
#if DEBUG
            _localytics = new LocalyticsSession("084443c212918bc1314eed4-c54e6f14-a2ef-11e2-9a95-00c76edb34ae");
#else
            _localytics = new LocalyticsSession("a100e3d768f37ed322e953f-64164842-a2eb-11e2-f180-0086c15f90fa");
#endif
            _localytics.open();
            _localytics.upload();
        }
    }
}
