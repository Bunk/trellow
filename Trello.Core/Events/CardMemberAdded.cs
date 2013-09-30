namespace Trellow.Events
{
    public class CardMemberAdded
    {
        public string CardId { get; set; }

        public string MemberId { get; set; }

        public string FullName { get; set; }
        
        public string Username { get; set; }

        public string Email { get; set; }

        public string AvatarHash { get; set; }
    }
}