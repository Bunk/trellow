using System.Collections.Generic;
using trello.ViewModels;
using trello.ViewModels.Cards;

namespace trello.Services.Messages
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
