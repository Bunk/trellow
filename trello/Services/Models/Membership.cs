using Newtonsoft.Json;

namespace trello.Services.Models
{
    public class Membership
    {
        public string Id { get; set; }

        [JsonProperty("idMember")]
        public string MemberId { get; set; }

        public string MemberType { get; set; }

        public bool Deactivated { get; set; }
    }
}
