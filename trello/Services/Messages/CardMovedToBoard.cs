namespace trello.Services.Messages
{
    public class CardMovedToBoard
    {
        public string CardId { get; set; }

        public string BoardId { get; set; }

        public string BoardName { get; set; }

        public string ListId { get; set; }

        public string ListName { get; set; }
    }
}