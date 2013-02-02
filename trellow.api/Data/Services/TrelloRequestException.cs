using System;

namespace trellow.api.Data.Services
{
    public class TrelloRequestException : Exception
    {
        public TrelloRequestException()
        {
        }

        public TrelloRequestException(string message) : base(message)
        {
        }

        public TrelloRequestException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}