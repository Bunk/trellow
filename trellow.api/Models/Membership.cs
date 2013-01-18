namespace trellow.api.Models
{
    public class Membership
    {
        public string Id { get; set; }

        public string IdMember { get; set; }

        public string MemberType { get; set; }

        public bool Deactivated { get; set; }
    }
}
