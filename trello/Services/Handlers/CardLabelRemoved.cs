using trellow.api.Cards;

namespace trello.Services.Handlers
{
    public class CardLabelRemoved
    {
        public string CardId { get; set; }

        public Color Color { get; set; }

        public string Name { get; set; }
    }

    public class CardCommented
    {
        public string CardId { get; set; }

        public string MemberId { get; set; }

        public string Text { get; set; }
    }
}
