using System;

namespace trellow.api.OAuth
{
    public class OAuthToken
    {
        public OAuthToken() { }
        public OAuthToken(string publicKey, string privateKey)
        {
            Key = publicKey;
            Secret = privateKey;
            IssuedDate = DateTime.UtcNow;
        }

        public string Key { get; set; }

        public string Secret { get; set; }

        public DateTime IssuedDate { get; set; }
    }
}