using System;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;

namespace trello.Services
{
    public class Token
    {
        public string Key { get; set; }

        public string Secret { get; set; }
    }

    public interface IOAuthClient
    {
        bool ValidateAccessToken();

        Task<Uri> GetLoginUri();

        Task<Token> GetAccessToken(string verifier);

        IRestClient GetRestClient();
    }

    public class TrelloOAuthClient : IOAuthClient
    {
        private const string ApplicationName = "Trellow";

        private string ConsumerKey = "69d9b907713f98fce88b772243734ee1";
        private string ConsumerSecret = "1fb29637cc712d8622aeac07fccf2e5caf1a713029032e96b9db2527ab14d65b";

        private Token _requestToken;
        private Token _accessToken;

        public string AccessToken
        {
            get { return _accessToken.Secret; }
        }

        public bool ValidateAccessToken()
        {
            if (_accessToken == null)
                return false;

            // todo: attempt to query something in the API to see if it works

            return true;
        }

        public async Task<Uri> GetLoginUri()
        {
            var client = GetRestClient(OAuth1Authenticator.ForRequestToken(ConsumerKey,
                                                                           ConsumerSecret,
                                                                           "http://localhost/oauthcallback"));

            var response = await client.ExecuteAwaitable(new RestRequest("OAuthGetRequestToken"));
            var query = response.ParseQueryString();
            _requestToken = new Token
            {
                Key = query["oauth_token"],
                Secret = query["oauth_token_secret"]
            };

            // https://trello.com/card/document-all-the-parameters-you-can-pass-to-authorize-aka-oauthauthorizetoken/4ed7e27fe6abb2517a21383d/95
            var redirectUri = client.BuildUri(new RestRequest("OAuthAuthorizeToken")
                                                  .AddParameter("oauth_token", _requestToken.Key)
                                                  .AddParameter("name", ApplicationName)
                                                  .AddParameter("scope", "read,write")
                                                  .AddParameter("expiration", "never")); //never, 30days

            return redirectUri;
        }

        public async Task<Token> GetAccessToken(string verifier)
        {
            var client = GetRestClient(OAuth1Authenticator.ForAccessToken(ConsumerKey,
                                                                          ConsumerSecret,
                                                                          _requestToken.Key,
                                                                          _requestToken.Secret,
                                                                          verifier));

            var response = await client.ExecuteAwaitable(new RestRequest("OAuthGetAccessToken"));
            var query = response.ParseQueryString();
            _accessToken = new Token
            {
                Key = query["oauth_token"],
                Secret = query["oauth_token_secret"]
            };

            return _accessToken;
        }

        public IRestClient GetRestClient()
        {
            var client = new RestClient
            {
                BaseUrl = "https://api.trello.com/1",
                Authenticator = OAuth1Authenticator.ForProtectedResource(ConsumerKey,
                                                                         ConsumerSecret,
                                                                         _accessToken.Key,
                                                                         _accessToken.Secret)
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