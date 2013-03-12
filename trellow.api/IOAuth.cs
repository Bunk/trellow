using System;
using System.Threading.Tasks;
using trellow.api.OAuth;

namespace TrelloNet
{
    public interface IOAuth
    {
        Task<Uri> GetAuthorizationUri(string applicationName, Scope scope, Expiration expiration, Uri callbackUri = null);

        Task<OAuthToken> Verify(string verifier);

        void Authorize(OAuthToken accessToken);

        void Deauthorize();
    }
}