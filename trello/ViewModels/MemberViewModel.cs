using trello.Extensions;
using trellow.api.Actions;
using trellow.api.Members;

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
            ImageUriThumb = member.AvatarHash.ToAvatarUrl(AvatarSize.Thumb);
            ImageUriLarge = member.AvatarHash.ToAvatarUrl(AvatarSize.Portrait);
        }

        public MemberViewModel(Action.ActionMember memberCreator)
        {
            Id = memberCreator.Id;
            FullName = memberCreator.FullName;
            Username = memberCreator.Username;
            ImageUriThumb = memberCreator.AvatarHash.ToAvatarUrl(AvatarSize.Thumb);
            ImageUriLarge = memberCreator.AvatarHash.ToAvatarUrl(AvatarSize.Portrait);
        }
    }
}