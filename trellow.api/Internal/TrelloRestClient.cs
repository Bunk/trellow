﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using trellow.api.Data.Services;
using trellow.api.OAuth;

namespace TrelloNet.Internal
{
    public abstract class BaseRestClient : RestClient
    {
        protected BaseRestClient() { }
        protected BaseRestClient(string baseUrl) : base(baseUrl) { }

#if !WINDOWS_PHONE
		public void Request(IRestRequest request)
		{
            var response = Execute(request);

			ThrowIfRequestWasUnsuccessful(request, response);
		}

		public T Request<T>(IRestRequest request) where T : class, new()
		{
			var response = Execute<T>(request);

			ThrowIfRequestWasUnsuccessful(request, response);

			return response.StatusCode == HttpStatusCode.NotFound ? null : response.Data;
		}
#endif

        public Task<IRestResponse> RequestAsync(IRestRequest request)
        {
            var tcs = new TaskCompletionSource<IRestResponse>();

            ExecuteAsync(request, (response, handle) =>
            {
                try
                {
                    ThrowIfRequestWasUnsuccessful(request, response);
                    tcs.SetResult(response);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });

            return tcs.Task;
        }

        public Task<T> RequestAsync<T>(IRestRequest request) where T : class, new()
        {
            var tcs = new TaskCompletionSource<T>();

            ExecuteAsync<T>(request, (response, handle) =>
            {
                try
                {
                    ThrowIfRequestWasUnsuccessful(request, response);
                    tcs.SetResult(response.StatusCode == HttpStatusCode.NotFound ? null : response.Data);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });

            return tcs.Task;
        }

        public Task<IEnumerable<T>> RequestListAsync<T>(IRestRequest request)
        {
            var tcs = new TaskCompletionSource<IEnumerable<T>>();

            ExecuteAsync<List<T>>(request, (response, handle) =>
            {
                try
                {
                    ThrowIfRequestWasUnsuccessful(request, response);
                    tcs.SetResult(response.StatusCode == HttpStatusCode.NotFound ? null : response.Data);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });

            return tcs.Task;
        }

        private void ThrowIfRequestWasUnsuccessful(IRestRequest request, IRestResponse response)
        {
            Debug.WriteLine(BuildUri(request));
            Debug.WriteLine(response.Content);

            // If PUT, POST or DELETE and not found, we'll throw, but for GET it's fine.
            if (request.Method == Method.GET && response.StatusCode == HttpStatusCode.NotFound)
                return;

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new TrelloUnauthorizedException(response.Content);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new TrelloException(response.Content);
        }
    }

    public class OAuthRestClient : BaseRestClient
    {
        private readonly string _publicKey;
        private readonly string _privateKey;
        private OAuthToken _requestToken;
        private OAuthToken _accessToken;

        public OAuthRestClient() { }
        public OAuthRestClient(string baseUrl, OAuthToken apiKey) : base(baseUrl)
        {
            _publicKey = apiKey.Key;
            _privateKey = apiKey.Secret;
        }
        public OAuthRestClient(string baseUrl, OAuthToken apiKey, OAuthToken accessToken) : base(baseUrl)
        {
            _publicKey = apiKey.Key;
            _privateKey = apiKey.Secret;

            AuthorizeAccess(accessToken);
        }

        public async Task<Uri> GetAuthorizationUri(string applicationName, Scope scope, Expiration expiration, Uri callbackUri = null)
        {
            callbackUri = callbackUri ?? new Uri("http://localhost/oauthcallback", UriKind.Absolute);

            Authenticator = OAuth1Authenticator.ForRequestToken(_publicKey, _privateKey, callbackUri.AbsoluteUri);

            var result = await RequestAsync(new RestRequest("OAuthGetRequestToken"));

            var query = result.Content.ParseQueryString();
            _requestToken = BuildOAuthToken(query);
            if (_requestToken == null)
                return null;

            return BuildUri(new RestRequest("OAuthAuthorizeToken")
                                .AddParameter("oauth_token", _requestToken.Key)
                                .AddParameter("name", applicationName)
                                .AddParameter("scope", scope.ToScopeString())
                                .AddParameter("expiration", expiration.ToExpirationString()));
        }

        public async Task<OAuthToken> AuthorizeRequest(string verifier)
        {
            Authenticator = OAuth1Authenticator.ForAccessToken(_publicKey, _privateKey, _requestToken.Key,
                                                               _requestToken.Secret, verifier);

            var response = await RequestAsync(new RestRequest("OAuthGetAccessToken"));
            
            var query = response.Content.ParseQueryString();
            _accessToken = BuildOAuthToken(query);
            if (_accessToken != null)
                AuthorizeAccess(_accessToken);

            return _accessToken;
        }

        public void AuthorizeAccess(OAuthToken token)
        {
            Authenticator = OAuth1Authenticator.ForProtectedResource(_publicKey, _privateKey, token.Key, token.Secret);
        }

        private static OAuthToken BuildOAuthToken(Dictionary<string, string> response)
        {
            string token;
            string secret;
            response.TryGetValue("oauth_token", out token);
            response.TryGetValue("oauth_token_secret", out secret);

            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(secret))
                return null;

            return new OAuthToken
            {
                Key = token,
                Secret = secret,
                IssuedDate = DateTime.UtcNow
            };
        }
    }

	public class TrelloRestClient : OAuthRestClient
	{
		private const string BASE_URL = "https://trello.com/1";

        public TrelloRestClient(OAuthToken apiKey)
			: base(BASE_URL, apiKey)
		{
			AddHandler("application/json", new TrelloDeserializer());
		}

        public TrelloRestClient(OAuthToken apiKey, OAuthToken accessToken)
            : base(BASE_URL, apiKey, accessToken)
        {
            AddHandler("application/json", new TrelloDeserializer());
        }
	}
}