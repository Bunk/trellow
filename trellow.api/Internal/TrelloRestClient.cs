using trellow.api;
using trellow.api.OAuth;

namespace TrelloNet.Internal
{
    public class TrelloRestClient : OAuthRestClient
	{
        public TrelloRestClient(ITrelloApiSettings settings)
            : base(settings.ApiRoot, new OAuthToken(settings.ApiConsumerKey, settings.ApiConsumerSecret))
        {
            AddHandler("application/json", new TrelloDeserializer());

            if (settings.AccessToken != null)
                Authorize(settings.AccessToken);
        }
	}
}