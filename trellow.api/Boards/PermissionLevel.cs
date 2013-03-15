using Newtonsoft.Json;
using trellow.api.Internal;

namespace trellow.api.Boards
{
	[JsonConverter(typeof(TrelloEnumConverter))]
	public enum PermissionLevel
	{
		Unknown = 0,
		Private,
		Org,
		Public
	}
}