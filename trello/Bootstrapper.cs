using System;
using System.Collections.Generic;
using System.Diagnostics;
using Caliburn.Micro;
using Microsoft.Phone.Controls;
using trello.Services;
using trello.Services.Data;
using trello.Services.OAuth;
using trello.ViewModels;

namespace trello
{
    public class ApplicationBootstrapper : PhoneBootstrapper
    {
        private PhoneContainer _container;

        public PhoneContainer Container
        {
            get { return _container; }
        }

        protected override void Configure()
        {
            _container = new PhoneContainer(RootFrame);
            _container.RegisterPhoneServices();

            _container.Instance(RootFrame);
            
            _container.Singleton<IProgressService, ProgressService>();
            _container.Singleton<IOAuthClient, TrelloOAuthClient>();
            _container.Singleton<IRequestProcessor, ProgressAwareRequestProcessor>();
            _container.Singleton<ITrelloSettings, TrelloSettings>();

            _container.Singleton<IBoardService, BoardService>();
            _container.Singleton<ICardService, CardService>();
            _container.Singleton<IProfileService, ProfileService>();

            _container.Singleton<SplashViewModel>();
            _container.Singleton<ShellViewModel>();
            _container.Singleton<BoardListViewModel>();
            _container.Singleton<CardListViewModel>();
            _container.Singleton<MessageListViewModel>();
            _container.Singleton<ProfileViewModel>();
            
            TelerikConventions.Install();
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

        protected override void OnUnhandledException(object sender, System.Windows.ApplicationUnhandledExceptionEventArgs e)
        {
            Debugger.Break();
            base.OnUnhandledException(sender, e);
        }
    }

    public class Bootstrapper : ApplicationBootstrapper
    {
    }
}
