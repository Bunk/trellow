using trellow.api.Checklists;

namespace trello.Services.Handlers
{
    public class CheckItemCreationRequested
    {
        public string ChecklistId { get; set; }

        public string Name { get; set; }
    }

    public class CheckItemCreated
    {
        public CheckItem CheckItem { get; set; }

        public string ChecklistId { get; set; }
    }
}
