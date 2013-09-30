namespace Trellow.Events
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

        /// <summary>
        /// Creates a new priority changed event with the correct 'pos' field based on the new card's 
        /// index relative to other cards in the list.
        /// </summary>
        public static CardPriorityChanged Create(string cardId, int index, double[] otherCardPositions)
        {
            var evt = new CardPriorityChanged
            {
                CardId = cardId
            };

            if (index <= 0)
            {
                evt.Type = PositionType.Top;
            }
            else if (index >= otherCardPositions.Length - 1)
            {
                evt.Type = PositionType.Bottom;
            }
            else
            {
                var prev = otherCardPositions[index - 1];
                var next = otherCardPositions[index + 1];
                evt.Type = PositionType.Exact;
                evt.Pos = ((prev + next) / 2);
            }
            return evt;
        }
    }
}
