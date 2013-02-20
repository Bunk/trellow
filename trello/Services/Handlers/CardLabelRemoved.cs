using TrelloNet;

namespace trello.Services.Handlers
{
    public class CardLabelRemoved
    {
        public string CardId { get; set; }

        public Color Color { get; set; }

        public string Name { get; set; }
    }
}
