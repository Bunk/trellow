﻿namespace trello.Services.Messages
{
    public class CheckItemRemoved
    {
        public string ChecklistId { get; set; }

        public string CheckItemId { get; set; }
    }
}
