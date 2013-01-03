using System;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Controls;
using Strilanc.Value;
using trello.Services.Data;
using trello.Services.OAuth;
using trello.Views;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public class SplashViewModel : Screen
    {
        private readonly INavigationService _navigationService;
        private readonly IOAuthClient _oauthClient;

        public SplashViewModel(INavigationService navigationService, IOAuthClient oauthClient)
        {
            _navigationService = navigationService;
            _oauthClient = oauthClient;
        }

        protected override void OnViewLoaded(object view)
        {
            // this is where we check for being logged in
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
            }
        }

        [UsedImplicitly]
        public void BrowserNavigating(NavigatingEventArgs args)
        {
            if (args.Uri.Host.Equals("localhost"))
            {
                // We've been redirected back w/ the token
                UsingView(async view =>
                {
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
