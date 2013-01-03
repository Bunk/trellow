using JetBrains.Annotations;
using trello.Services.OAuth;

namespace trello.Services
{
    public interface ITrelloSettings
    {
        string AppName { get; }

        string ApiConsumerKey { get; }

        string ApiConsumerSecret { get; }

        string OAuthScope { get; }

        string OAuthExpiration { get; }

        Token AccessToken { get; set; }
    }

    [UsedImplicitly]
    public class TrelloSettings : AppSettingsBase, ITrelloSettings
    {
        public string AppName
        {
            get { return "Trellow"; }
        }

        public string ApiConsumerKey
        {
            get { return "69d9b907713f98fce88b772243734ee1"; }
        }

        public string ApiConsumerSecret
        {
            get { return "1fb29637cc712d8622aeac07fccf2e5caf1a713029032e96b9db2527ab14d65b"; }
        }

        public string OAuthScope
        {
            get { return "read,write,account"; }
        }

        public string OAuthExpiration
        {
            get { return "never"; }
        }

        public Token AccessToken
        {
            get { return GetOrDefault<Token>("AccessToken"); }
            set { Set("AccessToken", value); }
        }
    }
}
