using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using RestSharp;
using Trellow.Diagnostics;
using trellow.api;

namespace Trellow.Services.Network
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
                return TrelloError(ex, new RestResponse());
            }
            catch (Exception ex)
            {
                Analytics.LogException(ex, BuildKvp(request));
                return NoResults(ex, new RestResponse());
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
                return TrelloError(ex, default(T));
            }
            catch (Exception ex)
            {
                return NoResults(ex, default(T));
            }
        }

        public async Task<IEnumerable<T>> RequestListAsync<T>(IRestRequest request)
        {
            try
            {
                var value = await _client.RequestListAsync<T>(request);
                return value ?? Enumerable.Empty<T>();
            }
            catch (TrelloException ex)
            {
                return TrelloError(ex, Enumerable.Empty<T>());
            }
            catch (Exception ex)
            {
                return NoResults(ex, Enumerable.Empty<T>());
            }
        }

        public async Task<Uri> GetAuthorizationUri(string applicationName, Scope scope, Expiration expiration, Uri callbackUri = null)
        {
            try
            {
                return await _client.GetAuthorizationUri(applicationName, scope, expiration, callbackUri);
            }
            catch (Exception ex)
            {
                return ApiError<Uri>(ex);
            }
        }

        public async Task<OAuthToken> Verify(string verifier)
        {
            try
            {
                return await _client.Verify(verifier);
            }
            catch (Exception ex)
            {
                return ApiError<OAuthToken>(ex);
            }
        }

        public void Authorize(OAuthToken accessToken)
        {
            try
            {
                _client.Authorize(accessToken);
            }
            catch (Exception ex)
            {
                ApiError(ex);
            }
        }

        public void Deauthorize()
        {
            try
            {
                _client.Deauthorize();
            }
            catch (Exception ex)
            {
                ApiError(ex);
            }
        }

        private static T NoResults<T>(Exception ex, T value)
        {
            const string message = "There was an error making the request.  Please " +
                                   "ensure that you have an active internet connection.";

            Analytics.LogException(ex, new Dictionary<string, string>
            {
                { "Message", message },
                { "Type", "API-" + typeof(T).Name }
            });

            MessageBox.Show(message);
            
            return value;
        }

        private static T ApiError<T>(Exception ex, T value = default(T))
        {
            ApiError(ex);
            return value;
        }

        private static void ApiError(Exception ex)
        {
            const string message = "Could not contact the Trello API.  Please " +
                                   "ensure that you have an active internet connection.";

            Analytics.LogWarning(ex, new Dictionary<string, string>
            {
                { "Message", message },
                { "Type", "API" }
            });

            MessageBox.Show(message);
        }

        private static T TrelloError<T>(TrelloException ex, T value)
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

            Analytics.LogException(ex, new Dictionary<string, string>
            {
                { "Message", message },
                { "Type", "API-" + typeof(T).Name }
            });

            MessageBox.Show(message);

            return value;
        }

        private static Dictionary<string, string> BuildKvp(IRestRequest request)
        {
            var props = new Dictionary<string, string>
            {
                {"Uri", request.Resource},
                {"Method", request.Method.ToString()}
            };

            AugmentParametersInProps(props, request.Parameters);

            return props;
        }

        private static void AugmentParametersInProps(Dictionary<string, string> props, IEnumerable<Parameter> parameters)
        {
            foreach (var parm in parameters)
            {
                props[string.Format("{0}<{1}>", parm.Name, parm.Type)] = parm.Value.ToString();
            }
        }
    }
}