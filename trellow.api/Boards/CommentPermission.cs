using Newtonsoft.Json;
using trellow.api.Internal;

namespace trellow.api.Boards
{
	[JsonConverter(typeof(TrelloEnumConverter))]
	public enum CommentPermission
	{
		Unknown = 0,
		Disabled,
		Members,
		Org,
		Public		
	}
}