using trellow.api.Boards;

namespace trellow.api
{
    public class BoardName : IBoardId
	{
		public string Id { get; set; }
		public string Name { get; set; }

		public string GetBoardId()
		{
			return Id;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}