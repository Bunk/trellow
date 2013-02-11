using System;

namespace trello.Services.Handlers
{
    public class CardDueDateChanged
    {
        public string CardId { get; set; }

        public DateTime? DueDate { get; set; }
    }
}
