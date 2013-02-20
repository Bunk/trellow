using System;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Phone.Shell;
using trellow.api;

namespace trello.ViewModels
{
    public interface IConfigureTheAppBar
    {
        ApplicationBar ConfigureTheAppBar(ApplicationBar existing);
    }

    public abstract class PivotItemViewModel : Screen
    {
    }

    public abstract class PivotViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private readonly PivotFix<IScreen> _pivotFix;

        private ApplicationBar _appBar;

        protected readonly ITrelloApiSettings Settings;

        protected readonly INavigationService Navigation;

        public ApplicationBar AppBar
        {
            get { return _appBar; }
            set
            {
                _appBar = value;
                NotifyOfPropertyChange(() => AppBar);
            }
        }

        public PivotViewModel(ITrelloApiSettings settings, INavigationService navigation)
        {
            Settings = settings;
            Navigation = navigation;
            _pivotFix = new PivotFix<IScreen>(this);
        }

        protected override void OnViewLoaded(object view)
        {
            _pivotFix.OnViewLoaded(view, base.OnViewLoaded);
        }

        protected override void ChangeActiveItem(IScreen newItem, bool closePrevious)
        {
            if (AppBar != null)
                AppBar.IsVisible = false;

            _pivotFix.ChangeActiveItem(newItem, closePrevious, base.ChangeActiveItem);
        }

        protected override void OnActivationProcessed(IScreen item, bool success)
        {
            base.OnActivationProcessed(item, success);

            var appbar = BuildDefaultAppBar();

            var screen = item as IConfigureTheAppBar;
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