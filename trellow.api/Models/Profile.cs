using System.Collections.Generic;

namespace trellow.api.Models
{
    public class Profile
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Bio { get; set; }

        public string AvatarHash { get; set; }

        public string Status { get; set; }

        public List<Board> Boards { get; set; }

        public List<List> Lists { get; set; }

        public List<Card> Cards { get; set; }

        public List<string> Trophies { get; set; }

        public Profile()
        {
            Boards = new List<Board>();
            Lists = new List<List>();
            Cards = new List<Card>();
            Trophies = new List<string>();
        }
    }
}
