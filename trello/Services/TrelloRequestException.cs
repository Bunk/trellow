using System;

namespace trello.Services
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