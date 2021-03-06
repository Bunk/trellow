using RestSharp;
using trellow.api.Internal;

namespace trellow.api.Lists.Internal
{
	internal class ListsAddRequest : RestRequest
	{
		public ListsAddRequest(NewList list)
			: base("lists", Method.POST)
		{
			Guard.NotNull(list, "list");
			Guard.RequiredTrelloString(list.Name, "name");

			AddParameter("name", list.Name);
			AddParameter("idBoard", list.IdBoard.GetBoardId());	
		}
	}
}