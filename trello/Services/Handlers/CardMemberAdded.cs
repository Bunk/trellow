﻿namespace trello.Services.Handlers
{
    public class CardMemberAdded
    {
        public string CardId { get; set; }

        public string MemberId { get; set; }
    }

    public class CardMemberRemoved
    {
        public string CardId { get; set; }

        public string MemberId { get; set; }
    }
}