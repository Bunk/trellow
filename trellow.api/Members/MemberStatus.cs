using Newtonsoft.Json;
using trellow.api.Internal;

namespace trellow.api.Members
{
	[JsonConverter(typeof(TrelloEnumConverter))]
	public enum MemberStatus
	{
		Unknown = 0,
		Active,
		Idle,
		Disconnected
	}
}