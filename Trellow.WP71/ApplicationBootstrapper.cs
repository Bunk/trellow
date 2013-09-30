using System;
using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Trellow.Caliburn.Micro;
using Trellow.Diagnostics;
using Trellow.Services;
using Trellow.Services.Data;
using Trellow.Services.UI;
using Trellow.ViewModels.Notifications;
using trellow.api;
using trellow.api.Data;
using IApplicationBar = Trellow.Services.UI.IApplicationBar;

namespace Trellow
{
    public class ApplicationBootstrapper : PhoneBootstrapper
    {
        private PhoneContainer _container;

        protected override void Configure()
        {
            _container = new PhoneContainer();

            if (!Execute.InDesignMode)
                _container.RegisterPhoneServices(RootFrame);

            // Add the root frame to the container
            _container.Instance(RootFrame);

            // Register view models
            _container.AllTypesWhere(ViewModelsFilter, type => _container.RegisterPerRequest(type, null, type));
            _container.AllTransientTypesOf<NotificationViewModel>();

            // Services
            _container.PerRequest<IApplicationBar, DefaultApplicationBar>();
            _container.Singleton<ICache, FileSystemCache>();
            _container.Singleton<IFileReader, JsonResourceFileReader>();
            _container.Singleton<INetworkService, NetworkService>();
            _container.Singleton<IProgressService, ProgressService>();
            _container.Singleton<ITrelloApiSettings, TrelloSettings>();

            PhoneToolkitConventions.Install();
            TelerikConventions.Install();
            ApiConfiguration.Install(_container);
            EventHandlers.Install(_container);
        }

        private static bool ViewModelsFilter(Type type)
        {
            return type.Name.EndsWith("ViewModel") && !type.InNamespace("Notifications");
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

            Analytics.CreateAndStartAnalyticsSession();
        }

        protected override void OnClose(object sender, ClosingEventArgs e)
        {
            Analytics.CloseSession();

            base.OnClose(sender, e);
        }

        protected override void OnActivate(object sender, ActivatedEventArgs e)
        {
            base.OnActivate(sender, e);

            Analytics.CreateAndStartAnalyticsSession();
        }

        protected override void OnDeactivate(object sender, DeactivatedEventArgs e)
        {
            Analytics.CloseSession();

            base.OnDeactivate(sender, e);
        }

        protected override void OnUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            Analytics.LogException(e.ExceptionObject);

            base.OnUnhandledException(sender, e);
        }
    }
}