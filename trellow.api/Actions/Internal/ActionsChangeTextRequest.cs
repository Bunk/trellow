using RestSharp;
using trellow.api.Internal;

namespace trellow.api.Actions.Internal
{
	internal class ActionsChangeTextRequest : ActionsRequest
	{
        public ActionsChangeTextRequest(IActionId action, string newText)
            : base(action.GetActionId(), Method.PUT)
		{
            Guard.RequiredTrelloString(newText, "newText");
            AddParameter("text", newText);
		}
	}
}