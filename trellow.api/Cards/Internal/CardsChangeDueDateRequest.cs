using System;
using System.Globalization;
using RestSharp;

namespace TrelloNet.Internal
{
	internal class CardsChangeDueDateRequest : CardsRequest
	{
		public CardsChangeDueDateRequest(ICardId card, DateTimeOffset? due)
			: base(card, "due", Method.PUT)
		{
			var dueString = "";
            if (due.HasValue)
                dueString = due.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);
			this.AddValue(dueString);
		}
	}
}