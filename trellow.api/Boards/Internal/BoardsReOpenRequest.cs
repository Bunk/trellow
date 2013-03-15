using RestSharp;
using trellow.api.Internal;

namespace trellow.api.Boards.Internal
{
	internal class BoardsReOpenRequest : BoardsRequest
	{
		public BoardsReOpenRequest(IBoardId board)
			: base(board, "closed", Method.PUT)
		{
			this.AddValue("false");			
		}
	}
}