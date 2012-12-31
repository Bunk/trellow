using System;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using trello.Services.Data;

namespace trello.Services.OAuth
{
    public interface IOAuthClient
    {
        bool ValidateAccessToken();

        Task<Uri> GetLoginUri();

        Task<Token> GetAccessToken(string verifier);

        IRestClient GetRestClient();
    }

    public class TrelloOAuthClient : IOAuthClient
    {
        private readonly ITrelloSettings _settings;

        private Token _requestToken;

        public TrelloOAuthClient(ITrelloSettings settings)
        {
            _settings = settings;
        }

        public bool ValidateAccessToken()
        {
            if (_settings.AccessToken == null)
                return false;

            // todo: attempt to query something in the API to see if it works

            return true;
        }

        public async Task<Uri> GetLoginUri()
        {
            var client = GetRestClient(OAuth1Authenticator.ForRequestToken(_settings.ApiConsumerKey,
                                                                           _settings.ApiConsumerSecret,
                                                                           "http://localhost/oauthcallback"));

            var response = await client.ExecuteAwaitable(new RestRequest("OAuthGetRequestToken"));
            var query = response.ParseQueryString();
            _requestToken = new Token
            {
                Key = query["oauth_token"],
                Secret = query["oauth_token_secret"],
                IssuedDate = DateTime.UtcNow
            };

            // https://trello.com/card/document-all-the-parameters-you-can-pass-to-authorize-aka-oauthauthorizetoken/4ed7e27fe6abb2517a21383d/95
            var redirectUri = client.BuildUri(new RestRequest("OAuthAuthorizeToken")
                                                  .AddParameter("oauth_token", _requestToken.Key)
                                                  .AddParameter("name", _settings.AppName)
                                                  .AddParameter("scope", "read,write")
                                                  .AddParameter("expiration", "never")); //never, 30days

            return redirectUri;
        }

        public async Task<Token> GetAccessToken(string verifier)
        {
            var client = GetRestClient(OAuth1Authenticator.ForAccessToken(_settings.ApiConsumerKey,
                                                                          _settings.ApiConsumerSecret,
                                                                          _requestToken.Key,
                                                                          _requestToken.Secret,
                                                                          verifier));

            var response = await client.ExecuteAwaitable(new RestRequest("OAuthGetAccessToken"));
            var query = response.ParseQueryString();
            var accessToken = new Token
            {
                Key = query["oauth_token"],
                Secret = query["oauth_token_secret"],
                IssuedDate = DateTime.UtcNow
            };

            return (_settings.AccessToken = accessToken);
        }

        public IRestClient GetRestClient()
        {
            var client = new RestClient
            {
                BaseUrl = "https://api.trello.com/1",
                Authenticator = OAuth1Authenticator.ForProtectedResource(_settings.ApiConsumerKey,
                                                                         _settings.ApiConsumerSecret,
                                                                         _settings.AccessToken.Key,
                                                                         _settings.AccessToken.Secret)
            };
            return client;
        }

        private static RestClient GetRestClient(IAuthenticator authenticator)
        {
            var client = new RestClient
            {
                BaseUrl = "https://trello.com/1",
                Authenticator = authenticator
            };
            return client;
        }
    }
}