using TrelloNet;

namespace trello.Services.Handlers
{
    public class CardMemberAdded
    {
        public string CardId { get; set; }

        public Member Member { get; set; }
    }

    public class CardMemberRemoved
    {
        public string CardId { get; set; }

        public Member Member { get; set; }
    }
}
