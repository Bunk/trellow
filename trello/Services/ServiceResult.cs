using System.Collections.Generic;
using Newtonsoft.Json;

namespace trello.Services
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

    public class Preferences
    {
        public string PermissionLevel { get; set; }

        public string Voting { get; set; }

        public string Comments { get; set; }

        public string Invitations { get; set; }

        public bool SelfJoin { get; set; }

        public bool CardCovers { get; set; }
    }

    public class Invitation
    {
        
    }

    public class Membership
    {
        public string Id { get; set; }

        [JsonProperty("idMember")]
        public string MemberId { get; set; }

        public string MemberType { get; set; }

        public bool Deactivated { get; set; }
    }
}
