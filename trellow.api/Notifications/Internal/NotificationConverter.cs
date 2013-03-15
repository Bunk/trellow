using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using trellow.api.Actions;
using trellow.api.Internal;

namespace trellow.api.Notifications.Internal
{
	public class NotificationConverter : JsonCreationConverter<Notification>
	{
		private static readonly Dictionary<string, Func<JObject, Notification>> TypeMap = new Dictionary<string, Func<JObject, Notification>>
		{
			{ "addedToCard", _ => new AddedToCardNotification() },
			{ "invitedToBoard", _ => new InvitedToBoardNotification() },
			{ "addedToBoard", _ => new AddedToBoardNotification() },
			{ "addAdminToBoard", _ => new AddAdminToBoardNotification() },
			{ "addAdminToOrganization", _ => new AddAdminToOrganization() },
			{ "changeCard", CreateChangeCardNotification },
			{ "removedFromCard", _ => new RemovedFromCardNotification() },
			{ "removedFromBoard", _ => new RemovedFromBoardNotification() },
			{ "closeBoard", _ => new CloseBoardNotification() },
			{ "commentCard", _ => new CommentCardNotification() },
			{ "invitedToOrganization", _ => new InvitedToOrganizationNotification() },
			{ "removedFromOrganization", _ => new RemovedFromOrganizationNotification() },
			{ "mentionedOnCard", _ => new MentionedOnCardNotification() }
		};

		protected override Notification Create(Type objectType, JObject jObject)
		{
			Func<JObject, Notification> specificNotification;
		    return TypeMap.TryGetValue(ParseType(jObject), out specificNotification)
		               ? specificNotification(jObject)
		               : new Notification();
		}

        private static Notification CreateChangeCardNotification(JObject jObject)
        {
            if (jObject["data"]["listBefore"] != null)
                return new CardMovedNotification();

            var action = new ChangeCardNotification();
            ApplyUpdateData(action.Data, jObject);
            return action;
        }

        private static void ApplyUpdateData(IUpdateData updateData, JObject jObject)
        {
            var old = jObject["data"]["old"];
            if (old == null) return;

            var updatedProperty = ((JProperty)old.First).Name;
            updateData.Old = new Old
            {
                PropertyName = updatedProperty,
                Value = old[updatedProperty]
            };
        }

		private static string ParseType(JObject jObject)
		{
			return jObject["type"].ToObject<string>();
		}
	}
}