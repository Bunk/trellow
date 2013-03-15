using System.Collections.Generic;
using trellow.api.Boards;
using trellow.api.Cards;
using trellow.api.Organizations;

namespace trellow.api.Search
{
	public class SearchFilter
	{		
		public IEnumerable<IBoardId> Boards { get; set; }
		public IEnumerable<IOrganizationId> Organizations { get; set; }
		public IEnumerable<ICardId> Cards { get; set; }
	}
}