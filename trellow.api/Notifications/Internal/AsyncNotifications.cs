using System.Collections.Generic;
using System.Threading.Tasks;
using trellow.api.Internal;
using trellow.api.Members;

namespace trellow.api.Notifications.Internal
{
	internal class AsyncNotifications : IAsyncNotifications
	{
		private readonly IRequestClient _restClient;

		public AsyncNotifications(IRequestClient restClient)
		{
			_restClient = restClient;
		}

		public Task<Notification> WithId(string notificationId)
		{
			return _restClient.RequestAsync<Notification>(new NotificationsRequest(notificationId));
		}

		public Task<IEnumerable<Notification>> ForMe(IEnumerable<NotificationType> types = null, ReadFilter readFilter = ReadFilter.All, Paging paging = null)
		{
			return _restClient.RequestListAsync<Notification>(new NotificationsForMeRequest(new Me(), types, readFilter, paging));
		}
	}
}