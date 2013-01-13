using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RestSharp;
using RestSharp.Authenticators;
using Strilanc.Value;
using trello.Services.Data;

namespace trello.Services.OAuth
{
    public interface IOAuthClient
    {
        bool ValidateAccessToken();

        void Invalidate();

        Task<May<Uri>> GetLoginUri();

        Task<May<Token>> GetAccessToken(string verifier);

        IRestClient GetRestClient();
    }

    [UsedImplicitly]
    public class TrelloOAuthClient : IOAuthClient
    {
        private readonly ITrelloSettings _settings;

        private May<Token> _requestToken = new May<Token>();

        public TrelloOAuthClient(ITrelloSettings settings)
        {
            _settings = settings;
        }

        public bool ValidateAccessToken()
        {
            if (_settings.AccessToken == null)
                return false;

            // todo: attempt to query something in the API to see if it works
            // note: we're not expiring the token, so unless something blows up this will work

            return true;
        }

        public void Invalidate()
        {
            _settings.AccessToken = null;
        }

        public async Task<May<Uri>> GetLoginUri()
        {
            var client = GetRestClient(OAuth1Authenticator.ForRequestToken(_settings.ApiConsumerKey,
                                                                           _settings.ApiConsumerSecret,
                                                                           "http://localhost/oauthcallback"));

            var response = await client.ExecuteAwaitable(new RestRequest("OAuthGetRequestToken"));
            if (response.ResponseStatus != ResponseStatus.Completed ||
                response.StatusCode != HttpStatusCode.OK)
                throw new EndpointNotFoundException("Trello could not be contacted.  This application requires a " +
                                                    "valid internet connection, are you sure that you have internet " +
                                                    "connectivity?");

            var query = response.Content.ParseQueryString();
            _requestToken = BuildOAuthToken(query);

            // https://trello.com/card/document-all-the-parameters-you-can-pass-to-authorize-aka-oauthauthorizetoken/4ed7e27fe6abb2517a21383d/95
            return _requestToken
                .Select(token => client.BuildUri(new RestRequest("OAuthAuthorizeToken")
                                                     .AddParameter("oauth_token", token.Key)
                                                     .AddParameter("name", _settings.AppName)
                                                     .AddParameter("scope", _settings.OAuthScope)
                                                     .AddParameter("expiration", _settings.OAuthExpiration)));
        }

        public async Task<May<Token>> GetAccessToken(string verifier)
        {
            if (!_requestToken.HasValue)
                return May<Token>.NoValue;

            var token = _requestToken.ForceGetValue();
            var client = GetRestClient(OAuth1Authenticator.ForAccessToken(_settings.ApiConsumerKey,
                                                                          _settings.ApiConsumerSecret,
                                                                          token.Key,
                                                                          token.Secret,
                                                                          verifier));

            var response = await client.ExecuteAwaitable(new RestRequest("OAuthGetAccessToken"));
            var query = response.Content.ParseQueryString();
            var accessToken = BuildOAuthToken(query);

            accessToken.IfHasValueThenDo(t => _settings.AccessToken = t);

            return accessToken;
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

        private May<Token> BuildOAuthToken(Dictionary<string, string> response)
        {
            string token = null;
            string secret = null;
            response.TryGetValue("oauth_token", out token);
            response.TryGetValue("oauth_token_secret", out secret);

            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(secret))
                return May<Token>.NoValue;

            return new Token
            {
                Key = token,
                Secret = secret,
                IssuedDate = DateTime.UtcNow
            };
        }
    }

    [UsedImplicitly]
    public class MockOAuthClient : IOAuthClient
    {
        public bool ValidateAccessToken()
        {
            return true;
        }

        public void Invalidate()
        {
            
        }

        public Task<May<Uri>> GetLoginUri()
        {
            throw new NotSupportedException();
        }

        public Task<May<Token>> GetAccessToken(string verifier)
        {
            throw new NotSupportedException();
        }

        public IRestClient GetRestClient()
        {
            return new RestClient("https://api.trello.com/1");
        }
    }
}