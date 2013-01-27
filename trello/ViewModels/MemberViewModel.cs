using trellow.api.Models;

namespace trello.ViewModels
{
    public class MemberViewModel
    {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Username { get; set; }

        public string Bio { get; set; }

        public string ImageUriThumb { get; set; }

        public string ImageUriLarge { get; set; }

        public MemberViewModel(Member member)
        {
            Id = member.Id;
            FullName = member.FullName;
            Username = member.Username;
            Bio = member.Bio;
            ImageUriThumb = string.Format("https://trello-avatars.s3.amazonaws.com/{0}/30.png", member.AvatarHash);
            ImageUriLarge = string.Format("https://trello-avatars.s3.amazonaws.com/{0}/170.png", member.AvatarHash);
        }
    }
}