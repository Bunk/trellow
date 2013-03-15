using RestSharp;
using trellow.api.Internal;

namespace trellow.api.Lists.Internal
{
	internal class ListsUnarchiveRequest : ListsRequest
	{
		public ListsUnarchiveRequest(IListId list)
			: base(list, "closed", Method.PUT)
		{
			this.AddValue("false");			
		}
	}
}