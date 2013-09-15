namespace trello.Services.Messages
{
    public class CardMemberRemoved
    {
        public string CardId { get; set; }

        public string MemberId { get; set; }
    }
}