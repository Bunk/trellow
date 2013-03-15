using System.Net;

namespace trellow.api
{
	public class TrelloUnauthorizedException : TrelloException
	{
		public TrelloUnauthorizedException(string message) : base(message, HttpStatusCode.Unauthorized)
		{			
		}
	}
}