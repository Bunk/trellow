using System.Collections.Generic;
using trellow.api.Models;

namespace trello.Services.Handlers
{
    public class CardLabelsChanged
    {
        public string CardId { get; set; }

        public List<Label> Labels { get; set; }

        public CardLabelsChanged()
        {
            Labels = new List<Label>();
        }
    }
}
