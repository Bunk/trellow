namespace Trellow.Events
{
    public class ChecklistCreationRequested
    {
        public string CardId { get; set; }

        public string BoardId { get; set; }

        public string Name { get; set; }
    }
}