﻿namespace trello.Services.Messages
{
    public class CardCreationRequested
    {
        public string BoardId { get; set; }

        public string ListId { get; set; }

        public string Name { get; set; }
    }
}
