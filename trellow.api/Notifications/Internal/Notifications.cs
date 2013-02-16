using System.Collections.Generic;

#if !WINDOWS_PHONE
namespace TrelloNet.Internal
{
	internal class Notifications : INotifications
	{
		private readonly TrelloRestClient _restClient;

		public Notifications(TrelloRestClient restClient)
		{
			_restClient = restClient;
		}

		public Notification WithId(string notificationId)
		{
			return _restClient.Request<Notification>(new NotificationsRequest(notificationId));
		}

		public IEnumerable<Notification> ForMe(IEnumerable<NotificationType> types = null, ReadFilter readFilter = ReadFilter.All, Paging paging = null)
		{
			return _restClient.Request<List<Notification>>(new NotificationsForMeRequest(new Me(), types, readFilter, paging));
		}
	}
}
#endif