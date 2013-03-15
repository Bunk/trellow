using trellow.api.Checklists;
using trellow.api.Checklists.Internal;
using trellow.api.Internal;

namespace trellow.api.Cards.Internal
{
	internal class CardsForChecklistRequest : ChecklistsRequest
	{
		public CardsForChecklistRequest(IChecklistId checklist, CardFilter filter)
			: base(checklist, "cards")
		{
			this.AddCommonCardParameters();
			this.AddFilter(filter);
		}
	}
}