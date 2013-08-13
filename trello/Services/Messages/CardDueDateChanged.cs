using System;

namespace trello.Services.Messages
{
    public class CardDueDateChanged
    {
        public string CardId { get; set; }

        public DateTime? DueDate { get; set; }
    }
}
