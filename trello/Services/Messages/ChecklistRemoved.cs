namespace trello.Services.Messages
{
    public class ChecklistRemoved
    {
        public string CardId { get; set; }

        public string ChecklistId { get; set; }
    }
}