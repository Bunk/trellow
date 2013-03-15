using Newtonsoft.Json;
using trellow.api.Internal;

namespace trellow.api.Boards
{
	[JsonConverter(typeof(TrelloEnumConverter))]
	public enum InvitationPermission
	{
		Unknown = 0,
		Members,
		Owners,
        Admins
	}
}