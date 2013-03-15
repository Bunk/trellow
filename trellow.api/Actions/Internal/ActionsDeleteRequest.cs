using RestSharp;

namespace trellow.api.Actions.Internal
{
	internal class ActionsDeleteRequest : ActionsRequest
	{
        public ActionsDeleteRequest(IActionId action)
            : base(action.GetActionId(), Method.DELETE)
		{
		}
	}
}