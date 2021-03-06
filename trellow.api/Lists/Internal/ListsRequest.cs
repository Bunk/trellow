using RestSharp;
using trellow.api.Internal;

namespace trellow.api.Lists.Internal
{
	internal class ListsRequest : RestRequest
	{
		public ListsRequest(IListId list, string resource = "", Method method = Method.GET)
			: base("lists/{listId}/" + resource, method)
		{
			Guard.NotNull(list, "list");
			AddParameter("listId", list.GetListId(), ParameterType.UrlSegment);			
		}

		public ListsRequest(string listId, string resource = "", Method method = Method.GET) 
			: this(new ListId(listId), resource, method)
		{			
		}
	}
}