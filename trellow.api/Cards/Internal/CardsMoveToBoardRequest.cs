using RestSharp;
using trellow.api.Boards;
using trellow.api.Internal;
using trellow.api.Lists;

namespace trellow.api.Cards.Internal
{
	internal class CardsMoveToBoardRequest : CardsRequest
	{
		public CardsMoveToBoardRequest(ICardId card, IBoardId board, IListId list)
			: base(card, "idBoard", Method.PUT)
		{
			Guard.NotNull(board, "board");
			this.AddValue(board.GetBoardId());

			if (list != null)
				AddParameter("idList", list.GetListId());
		}
	}
}