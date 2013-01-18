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

        //public List<Invitation> Invitations { get; set; }

        //public List<Membership> Memberships { get; set; }

        public Board()
        {
            Lists = new List<List>();
            Cards = new List<Card>();
        }
    }

    public class List
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public bool Closed { get; set; }

        public int Pos { get; set; }

        public bool Subscribed { get; set; }
    }
}