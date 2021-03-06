using trellow.api.Checklists;

namespace trellow.api
{
	public class ChecklistName : IChecklistId
	{
		public string Id { get; set; }
		public string Name { get; set; }

		public string GetChecklistId()
		{
			return Id;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}