using RestSharp;

namespace trellow.api.Boards.Internal
{
	internal class BoardsMarkAsViewedRequest : BoardsRequest
	{
		public BoardsMarkAsViewedRequest(IBoardId board)
			: base(board, "markAsViewed", Method.POST)
		{
		}
	}
}