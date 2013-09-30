using System;

namespace Trellow
{
    public enum AvatarSize
    {
        Thumb, Portrait
    }

    public static class StringExtensions
    {
        public static Uri ToUri(this string str)
        {
            return string.IsNullOrWhiteSpace(str) ? null : new Uri(str, UriKind.Absolute);
        }

        public static string ToAvatarUrl(this string avatarHash, int size = 170)
        {
            return string.Format("https://trello-avatars.s3.amazonaws.com/{0}/{1}.png", avatarHash, size);
        }

        public static string ToAvatarUrl(this string avatarHash, AvatarSize size)
        {
            switch (size)
            {
                case AvatarSize.Portrait:
                    return ToAvatarUrl(avatarHash, 170);
                case AvatarSize.Thumb:
                    return ToAvatarUrl(avatarHash, 30);
                default:
                    return ToAvatarUrl(avatarHash);
            }
        }
    }
}