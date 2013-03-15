namespace trellow.api.Checklists
{
	public class CheckItem : ICheckItemId
	{
		public string Id { get; set; }
		public string Name { get; set; }

		public string GetCheckItemId()
		{
			return Id;
		}
	}
}