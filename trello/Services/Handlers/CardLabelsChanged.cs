using System.Collections.Generic;
using trellow.api.Models;

namespace trello.Services.Handlers
{
    public class CardLabelsChanged
    {
        public string CardId { get; set; }

        public List<Label> LabelsAdded { get; set; }

        public List<Label> LabelsRemoved { get; set; }

        public CardLabelsChanged()
        {
            LabelsAdded = new List<Label>();
            LabelsRemoved = new List<Label>();
        }
    }
}
