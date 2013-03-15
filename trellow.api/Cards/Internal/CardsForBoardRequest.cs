using trellow.api.Boards;
using trellow.api.Boards.Internal;
using trellow.api.Internal;

namespace trellow.api.Cards.Internal
{
	internal class CardsForBoardRequest : BoardsRequest
	{
		public CardsForBoardRequest(IBoardId board, BoardCardFilter filter)
			: base(board, "cards")
		{
			this.AddCommonCardParameters();
			this.AddFilter(filter);
		}
	}
}