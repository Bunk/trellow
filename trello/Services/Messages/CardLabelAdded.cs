using trellow.api.Cards;

namespace trello.Services.Messages
{
    public class CardLabelAdded
    {
        public string CardId { get; set; }

        public Color Color { get; set; }

        public string Name { get; set; }
    }
}