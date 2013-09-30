using trellow.api.Checklists;

namespace Trellow.Events
{
    public class CheckItemCreated
    {
        public CheckItem CheckItem { get; set; }

        public string ChecklistId { get; set; }
    }
}