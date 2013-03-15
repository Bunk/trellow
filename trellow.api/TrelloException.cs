using System;
using System.Net;

namespace trellow.api
{
	public class TrelloException : Exception
	{
        public HttpStatusCode StatusCode { get; private set; }

		public TrelloException(string message, HttpStatusCode statusCode) : base(message)
		{
		    StatusCode = statusCode;
		}
	}
}