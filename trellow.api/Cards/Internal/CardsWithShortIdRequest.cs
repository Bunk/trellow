using RestSharp;
using trellow.api.Boards;
using trellow.api.Boards.Internal;
using trellow.api.Internal;

namespace trellow.api.Cards.Internal
{
	internal class CardsWithShortIdRequest : BoardsRequest
	{
		public CardsWithShortIdRequest(int id, IBoardId board)
			: base(board, "cards/{cardId}")
		{
			AddParameter("cardId", id, ParameterType.UrlSegment);
			this.AddCommonCardParameters();
		}
	}
}