using System.Collections.Generic;
using System.Linq;
using RestSharp;
using trellow.api.Internal;
using trellow.api.Members;
using trellow.api.Members.Internal;

namespace trellow.api.Notifications.Internal
{
	internal class NotificationsForMeRequest : MembersRequest
	{
		public NotificationsForMeRequest(IMemberId member, IEnumerable<NotificationType> filter, ReadFilter readFilter, Paging paging)
			: base(member, "notifications")
		{
			AddTypeFilter(filter);
			this.AddFilter(readFilter, "read_filter");
			this.AddPaging(paging);
		}

		private void AddTypeFilter(IEnumerable<NotificationType> filters)
		{
			if (filters == null || !filters.Any())
				return;

			var filterString = string.Join(",", filters.Select(f => f.ToTrelloString()));
			AddParameter("filter", filterString, ParameterType.GetOrPost);
		}
	}
}