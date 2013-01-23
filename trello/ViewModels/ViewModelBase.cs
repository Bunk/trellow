using System;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Phone.Shell;
using trellow.api;

namespace trello.ViewModels
{
    public abstract class ViewModelBase : Screen
    {
        protected readonly ITrelloApiSettings Settings;
        protected readonly INavigationService Navigation;
        protected ApplicationBar _appBar;

        public ApplicationBar AppBar
        {
            get { return _appBar; }
            set
            {
                _appBar = value;
                NotifyOfPropertyChange(() => AppBar);
            }
        }

        public ViewModelBase(ITrelloApiSettings settings, INavigationService navigation)
        {
            Settings = settings;
            Navigation = navigation;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            var appbar = BuildDefaultAppBar();

            var screen = this as IConfigureTheAppBar;
            if (screen != null)
            {
                appbar = screen.ConfigureTheAppBar(appbar);
            }

            AppBar = appbar;
        }

        protected virtual ApplicationBar BuildDefaultAppBar()
        {
            var bar = new ApplicationBar {IsVisible = true, IsMenuEnabled = true, Opacity = 1};

            var accountSettings = new ApplicationBarMenuItem("profile");
            accountSettings.Click += (sender, args) => OpenProfile();
            bar.MenuItems.Add(accountSettings);

            var appSettings = new ApplicationBarMenuItem("settings");
            appSettings.Click += (sender, args) => OpenSettings();
            bar.MenuItems.Add(appSettings);

            var signout = new ApplicationBarMenuItem("sign out");
            signout.Click += (sender, args) => SignOut();
            bar.MenuItems.Add(signout);

            return bar;
        }

        private void OpenProfile()
        {
            Navigation.Navigate(new Uri("/Views/ProfileView.xaml", UriKind.Relative));
        }

        private void OpenSettings()
        {
            MessageBox.Show("Settings");
        }

        private void SignOut()
        {
            var result = MessageBox.Show(
                "All cached data will be removed from your phone and must be loaded again next time you sign in.\n\n" +
                "Do you really want to sign out?",
                "confirm sign out", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                Settings.AccessToken = null;
                Navigation.Navigate(new Uri("/Views/SplashView.xaml", UriKind.Relative));
            }
        }

        protected void UsingView<T>(Action<T> action) where T : class
        {
            var view = base.GetView() as T;
            if (view == null)
            {
                MessageBox.Show("The view could not be found.");
                return;
            }
            action(view);
        }
    }
}