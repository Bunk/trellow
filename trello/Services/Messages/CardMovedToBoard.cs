namespace trello.Services.Messages
{
    public class CardMovedToBoard
    {
        public string CardId { get; set; }

        public string BoardId { get; set; }
    }
}