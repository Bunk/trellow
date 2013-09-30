namespace Trellow.Events
{
    public class CheckItemChanged
    {
        public string CardId { get; set; }

        public string ChecklistId { get; set; }

        public string CheckItemId { get; set; }

        public bool Value { get; set; }
    }
}
