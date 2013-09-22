using System;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Phone.Shell;
using trellow.api;
using JetBrains.Annotations;

namespace trello.ViewModels
{
    public abstract class ViewModelBase : Screen
    {
        protected readonly ITrelloApiSettings Settings;
        protected readonly INavigationService Navigation;
        protected ApplicationBar _appBar;

        [UsedImplicitly]
        public ApplicationBar AppBar
        {
            get { return _appBar; }
            set
            {
                _appBar = value;
                NotifyOfPropertyChange(() => AppBar);
            }
        }

        protected ViewModelBase(ITrelloApiSettings settings, INavigationService navigation)
        {
            Settings = settings;
            Navigation = navigation;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            var appbar = BuildDefaultAppBar();

//            var screen = this as IConfigureTheAppBar;
//            if (screen != null)
//            {
//                appbar = screen.Configure(appbar);
//            }

            AppBar = appbar;
        }

        protected virtual ApplicationBar BuildDefaultAppBar()
        {
            var bar = new ApplicationBar {IsVisible = true, IsMenuEnabled = true, Opacity = 1};

            var accountSettings = new ApplicationBarMenuItem("profile");
            accountSettings.Click += (sender, args) => OpenProfile();
            bar.MenuItems.Add(accountSettings);

            var signout = new ApplicationBarMenuItem("sign out");
            signout.Click += (sender, args) => SignOut();
            bar.MenuItems.Add(signout);

            return bar;
        }

        private void OpenProfile()
        {
            Navigation.UriFor<ProfileViewModel>().Navigate();
        }

        private void SignOut()
        {
            var result = MessageBox.Show(
                "All cached data will be removed from your phone and must be loaded again next time you sign in.\n\n" +
                "Do you really want to sign out?",
                "confirm sign out", MessageBoxButton.OKCancel);
            if (result != MessageBoxResult.OK) return;

            Settings.AccessToken = null;
            Navigation.Navigate(new Uri("/Views/SplashView.xaml", UriKind.Relative));
        }
    }
}