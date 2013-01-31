using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Phone.Controls;
using trello.Services;
using trello.Services.Cache;
using trello.Services.Pipelines;
using trello.Services.Stages;
using trello.ViewModels;
using trellow.api;
using trellow.api.Data;
using trellow.api.Data.Services;
using trellow.api.Data.Stages;
using trellow.api.OAuth;

namespace trello
{
    public class ApplicationBootstrapper : PhoneBootstrapper
    {
        private PhoneContainer _container;

        protected override void Configure()
        {
            _container = new PhoneContainer(RootFrame);
            _container.RegisterPhoneServices();

            _container.Instance(RootFrame);

            _container.Singleton<ICache, FileSystemCache>();
            _container.Singleton<IProgressService, ProgressService>();

            _container.Singleton<ProgressIndicatorStage>();
            _container.Singleton<CacheStage>();
            _container.Singleton<CallExternalApiStage>();

            _container.Singleton<IRequestPipeline, ConnectedRequestPipeline>();
            _container.Singleton<IRequestProcessor, RequestProcessor>();
            _container.Singleton<ITrelloApiSettings, TrelloSettings>();

            _container.Singleton<SplashViewModel>();
            _container.Singleton<ShellViewModel>();
            _container.Singleton<BoardsViewModel>();
            _container.Singleton<CardListViewModel>();
            _container.Singleton<MessageListViewModel>();
            _container.Singleton<ProfileViewModel>();

            _container.PerRequest<BoardViewModel>();
            _container.PerRequest<BoardListViewModel>();
            _container.PerRequest<CardViewModel>();
            _container.PerRequest<CardDetailShellViewModel>();
            _container.PerRequest<CardDetailViewModel>();
            _container.PerRequest<ChecklistViewModel>();
            _container.PerRequest<ChecklistItemViewModel>();
            _container.PerRequest<AttachmentViewModel>();

            //RegisterJsonRepository(_container);
            RegisterRepository(_container);

            TelerikConventions.Install();
        }

        private static void RegisterJsonRepository(PhoneContainer container)
        {
            container.Singleton<IOAuthClient, MockOAuthClient>();

            container.Singleton<IBoardService, JsonBoardService>();
            container.Singleton<ICardService, JsonCardService>();
            container.Singleton<IProfileService, JsonProfileService>();
        }

        private static void RegisterRepository(PhoneContainer container)
        {
            container.Singleton<IOAuthClient, TrelloOAuthClient>();

            container.Singleton<IBoardService, BoardService>();
            container.Singleton<ICardService, CardService>();
            container.Singleton<IProfileService, ProfileService>();
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