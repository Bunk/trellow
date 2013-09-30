namespace Trellow.Events
{
    public class ChecklistCreated
    {
        public string CardId { get; set; }

        public string BoardId { get; set; }

        public string ChecklistId { get; set; }

        public string Name { get; set; }
    }
}