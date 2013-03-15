using System;
using trellow.api.Members;

namespace trellow.api.Notifications
{
	public class Notification : INotificationId
	{
		public string Id { get; set; }
		public bool Unread { get; set; }
		public DateTime Date { get; set; }
		public string IdMemberCreator { get; set; }
        public Member MemberCreator { get; set; }

        public Notification()
        {
            MemberCreator = new Member();
        }

		public string GetNotificationId()
		{
			return Id;
		}
	}
}