using trellow.api.Checklists;

namespace trello.Services.Messages
{
    public class CheckItemCreated
    {
        public CheckItem CheckItem { get; set; }

        public string ChecklistId { get; set; }
    }
}