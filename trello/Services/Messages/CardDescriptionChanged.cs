namespace trello.Services.Messages
{
    public class CardDescriptionChanged
    {
        public string CardId { get; set; }

        public string Description { get; set; }
    }
}