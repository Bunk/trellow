using trellow.api.Cards;

namespace trello.Services.Messages
{
    public class CardMovedToList
    {
        public Card Card { get; set; }

        public string SourceListId { get; set; }

        public string DestinationListId { get; set; }
    }
}
