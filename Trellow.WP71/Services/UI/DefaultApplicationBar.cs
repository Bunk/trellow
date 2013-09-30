using System;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Shell;
using Trellow.UI;
using Trellow.ViewModels;
using Trellow.ViewModels.Help;
using trellow.api;

namespace Trellow.Services.UI
{
    [UsedImplicitly]
    public class DefaultApplicationBar : PropertyChangedBase, IApplicationBar
    {
        private readonly INavigationService _navigation;
        
        private readonly ITrelloApiSettings _settings;
        
        private ApplicationBar _instance;

        public ApplicationBar Instance
        {
            get { return _instance; }
            private set
            {
                if (Equals(value, _instance)) return;
                _instance = value;
                NotifyOfPropertyChange(() => Instance);
            }
        }

        public DefaultApplicationBar(INavigationService navigation, ITrelloApiSettings settings)
        {
            _navigation = navigation;
            _settings = settings;
        }

        public void Update(ApplicationBar applicationBar)
        {
            Instance = applicationBar;
        }

        public void UpdateWith(Action<IBuildApplicationBarsWithDefaults> action)
        {
            var builder = new ApplicationBarBuilder()
                .WithDefaults(bar =>
                {
                    bar.AddMenuItem("profile", OpenProfile);
                    bar.AddMenuItem("sign out", SignOut);
                    bar.AddMenuItem("about", About);
                    bar.AddMenuItem("release notes", ReleaseNotes);
                });

            action(builder);

            Update(builder.Build());
        }

        private void About()
        {
            _navigation.UriFor<AboutViewModel>().Navigate();
        }

        private void OpenProfile()
        {
            _navigation.UriFor<ProfileViewModel>().Navigate();
        }

        private void SignOut()
        {
            var result = MessageBox.Show(
                "All cached data will be removed from your phone and must be loaded again next time you sign in.\n\n" +
                "Do you really want to sign out?",
                "confirm sign out", MessageBoxButton.OKCancel);
            if (result != MessageBoxResult.OK) return;

            _settings.AccessToken = null;
            _navigation.UriFor<SplashViewModel>().Navigate();
        } 

        private void ReleaseNotes()
        {
            _navigation.UriFor<ReleaseNotesViewModel>()
                       .WithParam(model => model.MinimumVersion, null)
                       .WithParam(model => model.MaximumVersion, null)
                       .Navigate();
        }
    }
}