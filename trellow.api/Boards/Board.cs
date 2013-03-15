using System.Collections.Generic;
using trellow.api.Cards;

namespace trellow.api.Boards
{
	public class Board : IBoardId, IUpdatableBoard
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Desc { get; set; }
		public bool Closed { get; set; }
		public string IdOrganization { get; set; }
		public bool Pinned { get; set; }
		public string Url { get; set; }
		public BoardPreferences Prefs { get; set; }
		public bool Invited { get; set; }
		public Dictionary<Color, string> LabelNames { get; set; }

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