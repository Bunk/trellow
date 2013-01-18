namespace trellow.api.Models
{
    public class Preferences
    {
        public string PermissionLevel { get; set; }

        public string Voting { get; set; }

        public string Comments { get; set; }

        public string Invitations { get; set; }

        public bool SelfJoin { get; set; }

        public bool CardCovers { get; set; }
    }
}