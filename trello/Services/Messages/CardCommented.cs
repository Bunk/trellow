﻿namespace trello.Services.Messages
{
    public class CardCommented
    {
        public string CardId { get; set; }

        public string MemberId { get; set; }

        public string Text { get; set; }
    }
}