using System.Collections.Generic;
using trellow.api.Actions;
using trellow.api.Boards;
using trellow.api.Cards;
using trellow.api.Members;
using trellow.api.Organizations;

namespace trellow.api.Search
{
	public class SearchResults
	{
		public SearchResults()
		{
			Boards = new List<Board>();
			Cards = new List<Card>();
			Organizations = new List<Organization>();
			Members = new List<Member>();
			Actions = new List<Action>();
		}

		public List<Board> Boards { get; set; }
		public List<Card> Cards { get; set; }
		public List<Organization> Organizations { get; set; }
		public List<Member> Members { get; set; }
		public List<Action> Actions { get; set; }		
	}
}