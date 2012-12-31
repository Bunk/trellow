using System;

namespace trello.Services.OAuth
{
    public class Token
    {
        public string Key { get; set; }

        public string Secret { get; set; }

        public DateTime IssuedDate { get; set; }
    }
}