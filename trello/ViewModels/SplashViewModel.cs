using System;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Controls;
using Strilanc.Value;
using trello.Services;
using trello.Views;
using trellow.api.Data;
using trellow.api.Data.Services;
using trellow.api.OAuth;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public class SplashViewModel : Screen
    {
        private readonly INavigationService _navigationService;
        private readonly IOAuthClient _oauthClient;
        private readonly ICache _cache;
        private string _status;

        public string Status
        {
            get { return _status; }
            set
            {
                if (value == _status) return;
                _status = value;
                NotifyOfPropertyChange(() => Status);
            }
        }

        public SplashViewModel(INavigationService navigationService, IOAuthClient oauthClient, ICache cache)
        {
            _navigationService = navigationService;
            _oauthClient = oauthClient;
            _cache = cache;
        }

        protected override async void OnViewLoaded(object view)
        {
            Status = "Loading...";

            Status = "Populating Cache...";
            var success = await _cache.Initialize();
            if (!success)
                Status = "Invalidating the cache...";

            Status = "Signing in...";
            var loggedin = _oauthClient.ValidateAccessToken();
            if (!loggedin)
            {
                // Not logged in, so let's initiate the OAuth v1 handshake
                Login();
            }
            else
            {
                // Already logged in, so let's go to the shell
                Continue();

                Status = "Signed in...";
            }
        }

        [UsedImplicitly]
        public void BrowserNavigating(NavigatingEventArgs args)
        {
            if (args.Uri.Host.Equals("localhost"))
            {
                args.Cancel = true;

                // We've been redirected back w/ the token
                UsingView(async view =>
                {
                    view.Browser.Visibility = Visibility.Collapsed;

                    var parms = args.Uri.Query.ParseQueryString();
                    var verifier = parms["oauth_verifier"];
                    var token = await _oauthClient.GetAccessToken(verifier);

                    token
                        .IfHasValueThenDo(t => Continue())
                        .ElseDo(Login);
                });
            }
        }

        private void Continue()
        {
            UsingView(view => view.Browser.Visibility = Visibility.Collapsed);
            _navigationService.Navigate(new Uri("/Views/ShellView.xaml", UriKind.Relative));
        }

        private async void Login()
        {
            // todo: Handle exceptions here so that we can give a good message to the user
            var loginUri = await _oauthClient.GetLoginUri();
            loginUri.IfHasValueThenDo(LoadLogin);
        }

        private void LoadLogin(Uri uri)
        {
            UsingView(async view =>
            {
                // clear the browser cookies so that we can re-login
                await view.Browser.ClearCookiesAsync();

                view.Browser.Navigate(uri);

                view.Browser.Visibility = Visibility.Visible;
            });
        }

        private void UsingView(Action<SplashView> action)
        {
            var view = base.GetView() as SplashView;
            if (view == null)
            {
                MessageBox.Show("There was a problem with logging into Trello.  " +
                                "Please verify that you can browse the internet.");
                return;
            }
            action(view);
        }
    }
}