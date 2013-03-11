using trellow.api;
using trellow.api.OAuth;

namespace TrelloNet.Internal
{
    public class TrelloRestClient : OAuthRestClient
	{
		private const string BASE_URL = "https://trello.com/1";

        public TrelloRestClient(ITrelloApiSettings settings)
            : base(BASE_URL, new OAuthToken(settings.ApiConsumerKey, settings.ApiConsumerSecret))
        {
            AddHandler("application/json", new TrelloDeserializer());

            if (settings.AccessToken != null)
                Authorize(settings.AccessToken);
        }
	}
}