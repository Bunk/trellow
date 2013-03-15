using RestSharp;
using trellow.api.Internal;

namespace trellow.api.Lists.Internal
{
	internal class ListsArchiveRequest : ListsRequest
	{
		public ListsArchiveRequest(IListId list)
			: base(list, "closed", Method.PUT)
		{
			this.AddValue("true");			
		}
	}
}