﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TrelloNet;
using trello.Services;
using trello.Services.Cache;
using trello.Services.Handlers;
using trello.Services.Stages;
using trello.ViewModels;
using trellow.api;
using trellow.api.Data;

namespace trello
{
    public class ApplicationBootstrapper : PhoneBootstrapper
    {
        private PhoneContainer _container;

        private CardDetailCommandHandler _detailHandler;

        protected override void Configure()
        {
            _container = new PhoneContainer(RootFrame);
            _container.RegisterPhoneServices();

            _container.Instance(RootFrame);

            _container.Singleton<ICache, FileSystemCache>();
            _container.Singleton<IProgressService, ProgressService>();
            _container.Singleton<INetworkService, NetworkService>();

            _container.Singleton<ProgressIndicatorStage>();
            _container.Singleton<CacheStage>();

            _container.Singleton<SplashViewModel>();
            _container.Singleton<ShellViewModel>();
            _container.Singleton<MyBoardsViewModel>();
            _container.Singleton<MyCardsViewModel>();
            _container.Singleton<MessageListViewModel>();
            _container.Singleton<ProfileViewModel>();

            _container.PerRequest<BoardViewModel>();
            _container.PerRequest<BoardListViewModel>();
            _container.PerRequest<CardViewModel>();
            _container.PerRequest<CardDetailViewModel>();
            _container.PerRequest<CardDetailPivotViewModel>();
            _container.PerRequest<CardDetailOverviewViewModel>();
            _container.PerRequest<CardDetailChecklistViewModel>();
            _container.PerRequest<ChecklistViewModel>();
            _container.PerRequest<ChecklistItemViewModel>();
            _container.PerRequest<AttachmentViewModel>();

            _container.Singleton<CardDetailCommandHandler>();

            _container.Singleton<ITrelloApiSettings, TrelloSettings>();
            _container.Handler<ITrello>(container =>
            {
                var settings = (ITrelloApiSettings) container.GetInstance(typeof (ITrelloApiSettings), null);
                var api = new Trello(settings.ApiConsumerKey, settings.ApiConsumerSecret);
                if (settings.AccessToken != null)
                    api.Authorize(settings.AccessToken);

                return api;
            });

            _container.Singleton<IApplicationBar, ApplicationBar>();

            TelerikConventions.Install();

            _detailHandler = (CardDetailCommandHandler) _container.GetInstance(typeof (CardDetailCommandHandler), null);
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

        protected override void OnUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // TODO: Handle these a little better.
            Debugger.Break();

            base.OnUnhandledException(sender, e);
        }
    }
}