namespace trello.Services.Handlers
{
    public enum PositionType
    {
        Top, Bottom, Exact
    }

    public class CardPriorityChanged
    {
        public string CardId { get; set; }

        public PositionType Type { get; set; }

        public double Pos { get; set; }
    }
}
