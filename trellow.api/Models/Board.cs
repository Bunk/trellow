using System.Collections.Generic;

namespace trellow.api.Models
{
    public class Board
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public bool Invited { get; set; }

        public bool Pinned { get; set; }

        public Preferences Prefs { get; set; }

        public List<List> Lists { get; set; }

        public List<Card> Cards { get; set; }

        public LabelNames LabelNames { get; set; }

        public Board()
        {
            Lists = new List<List>();
            Cards = new List<Card>();
            LabelNames = new LabelNames();
        }
    }

    public class LabelNames
    {
        public string Blue { get; set; }

        public string Green { get; set; }

        public string Orange { get; set; }

        public string Purple { get; set; }

        public string Red { get; set; }

        public string Yellow { get; set; }
    }
}