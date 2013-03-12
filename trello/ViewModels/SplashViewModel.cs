using System;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Controls;
using TrelloNet;
using trello.Services;
using trello.Views;
using trellow.api;
using trellow.api.Data.Services;
using trellow.api.OAuth;

namespace trello.ViewModels
{
    [UsedImplicitly]
    public class SplashViewModel : Screen
    {
        private readonly INavigationService _navigationService;
        private readonly ITrelloApiSettings _settings;
        private readonly ICache _cache;
        private readonly ITrello _api;
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

        public SplashViewModel(INavigationService navigationService, ITrelloApiSettings settings, ITrello api, ICache cache)
        {
            _navigationService = navigationService;
            _settings = settings;
            _cache = cache;
            _api = api;
        }

        protected override async void OnViewLoaded(object view)
        {
            Status = "Loading...";

            Status = "Populating Cache...";
            var success = await _cache.Initialize();
            if (!success)
                Status = "Invalidating the cache...";

#if DISCONNECTED
            _settings.AccessToken = new OAuthToken("publicKey", "privateKey");
#endif

            var validated = await _api.AccessTokenIsFresh(_settings.AccessToken);
            if (validated)
            {
                Status = "Signed in";
                AccessGranted(_settings.AccessToken);
            }
            else
            {
                Status = "Signing in...";
                AccessDenied();
            }
        }

        [UsedImplicitly]
        public void BrowserNavigating(NavigatingEventArgs args)
        {
            if (!args.Uri.Host.Equals("localhost")) 
                return;

            args.Cancel = true;

            // We've been redirected back w/ the token
            UsingView(async view =>
            {
                view.Browser.Visibility = Visibility.Collapsed;

                var parms = args.Uri.Query.ParseQueryString();
                var verifier = parms["oauth_verifier"];

                var token = await _api.Verify(verifier);
                if (token != null)
                {
                    AccessGranted(token);
                }
                else
                {
                    AccessDenied();
                }
            });
        }

        private async void AccessGranted(OAuthToken token)
        {
            _settings.AccessToken = token;

            var profile = await _api.Members.Me();
            if (profile != null)
            {
                _settings.MemberId = profile.Id;
                _settings.Username = profile.Username;
                _settings.Fullname = profile.FullName;
                _settings.AvatarHash = profile.AvatarHash;
            }

            UsingView(view => view.Browser.Visibility = Visibility.Collapsed);
            _navigationService.UriFor<ShellViewModel>().Navigate();
        }

        private async void AccessDenied()
        {
            var uri = await _api.GetAuthorizationUri("Trellow", Scope.ReadWriteAccount, Expiration.Never);
            if (uri != null)
            {
                LoadLogin(uri);
            }
            else
            {
                Status = "Could not sign you in.\n\nPlease ensure you have an active internet connection.";
            }
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