using System.Collections.Generic;
using trello.ViewModels;

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

        public static CardPriorityChanged Create(string cardId, int index, IList<CardViewModel> cards)
        {
            var evt = new CardPriorityChanged
            {
                CardId = cardId
            };

            if (index == 0)
            {
                evt.Type = PositionType.Top;
            }
            else if (index == cards.Count - 1)
            {
                evt.Type = PositionType.Bottom;
            }
            else
            {
                var prev = cards[index - 1].Pos;
                var next = cards[index + 1].Pos;
                evt.Type = PositionType.Exact;
                evt.Pos = ((prev + next) / 2);
            }
            return evt;
        }
    }
}
