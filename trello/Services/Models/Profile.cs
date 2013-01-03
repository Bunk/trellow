using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trello.Services.Models
{
    public class Profile
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Bio { get; set; }

        public string AvatarSource { get; set; }

        public string AvatarHash { get; set; }

        public string GravatarHash { get; set; }

        public string MemberType { get; set; }

        public string Status { get; set; }

        public bool Confirmed { get; set; }

        public List<string> Trophies { get; set; }

        public Profile()
        {
            Trophies = new List<string>();
        }
    }
}
