using System.Collections.Generic;
using TrelloNet;

namespace trello.Services.Handlers
{
    public class CardLabelsChanged
    {
        public string CardId { get; set; }

        public List<Card.Label> LabelsAdded { get; set; }

        public List<Card.Label> LabelsRemoved { get; set; }

        public CardLabelsChanged()
        {
            LabelsAdded = new List<Card.Label>();
            LabelsRemoved = new List<Card.Label>();
        }
    }
}
