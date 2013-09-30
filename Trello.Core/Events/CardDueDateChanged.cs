using System;

namespace Trellow.Events
{
    public class CardDueDateChanged
    {
        public string CardId { get; set; }

        public DateTime? DueDate { get; set; }
    }
}
