namespace trello.Services.Models
{
    public class Board
    {
        public string Id { get; set; }

        public string IdOrganization { get; set; }

        public string Name { get; set; }

        public string Desc { get; set; }

        public bool Closed { get; set; }

        public bool Invited { get; set; }

        public bool Pinned { get; set; }

        public string Url { get; set; }

        public Preferences Prefs { get; set; }

        //public List<Invitation> Invitations { get; set; }

        //public List<Membership> Memberships { get; set; }

        public Board() { }
    }
}