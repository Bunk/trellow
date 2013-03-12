using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using RestSharp;
using TrelloNet;
using TrelloNet.Internal;
using trellow.api.OAuth;

namespace trello.Services.Cache
{
    public class ErrorHandlingRestClient : IRequestClient
    {
        private readonly IRequestClient _client;

        public ErrorHandlingRestClient(IRequestClient client)
        {
            _client = client;
        }

        public async Task<IRestResponse> RequestAsync(IRestRequest request)
        {
            try
            {
                return await _client.RequestAsync(request);
            }
            catch (TrelloException ex)
            {
                return TrelloError(new RestResponse(), ex);
            }
            catch(Exception ex)
            {
                return NoResults(new RestResponse());
            }
        }

        public async Task<T> RequestAsync<T>(IRestRequest request) where T : class, new()
        {
            try
            {
                return await _client.RequestAsync<T>(request);
            }
            catch (TrelloException ex)
            {
                return TrelloError(default(T), ex);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public async Task<IEnumerable<T>> RequestListAsync<T>(IRestRequest request)
        {
            try
            {
                var value = await _client.RequestListAsync<T>(request);
                return value ?? NoResults(Enumerable.Empty<T>());
            }
            catch (TrelloException ex)
            {
                return TrelloError(Enumerable.Empty<T>(), ex);
            }
            catch (Exception ex)
            {
                return NoResults(Enumerable.Empty<T>());
            }
        }

        public async Task<Uri> GetAuthorizationUri(string applicationName, Scope scope, Expiration expiration, Uri callbackUri = null)
        {
            try
            {
                return await _client.GetAuthorizationUri(applicationName, scope, expiration, callbackUri);
            }
            catch
            {
                return ApiError<Uri>();
            }
        }

        public async Task<OAuthToken> Verify(string verifier)
        {
            try
            {
                return await _client.Verify(verifier);
            }
            catch
            {
                return ApiError<OAuthToken>();
            }
        }

        public void Authorize(OAuthToken accessToken)
        {
            try
            {
                _client.Authorize(accessToken);
            }
            catch
            {
                ApiError();
            }
        }

        public void Deauthorize()
        {
            try
            {
                _client.Deauthorize();
            }
            catch
            {
                ApiError();
            }
        }

        private static T NoResults<T>(T value)
        {
            MessageBox.Show("There was an error making the request.  Please " +
                            "ensure that you have an active internet connection.");
            return value;
        }

        private static T ApiError<T>(T value = default(T))
        {
            ApiError();
            return value;
        }

        private static void ApiError()
        {
            MessageBox.Show("Could not contact the Trello API.  Please " +
                            "ensure that you have an active internet connection.");
        }

        private static T TrelloError<T>(T value, TrelloException ex)
        {
            var message = "There was an error contacting the Trello servers.  Please " +
                          "ensure that you have an active internet connection.";
            switch (ex.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                {
                    message = "Trello says you are unauthorized to do that.";
                    break;
                }
                case HttpStatusCode.InternalServerError:
                {
                    if (ex.Message.Contains("Server overloaded"))
                        message = "The Trello servers are currently experiencing a large amount " +
                                  "of traffic.  Please try again later.";
                    break;
                }
            }

            MessageBox.Show(message);
            return value;
        }
    }
}