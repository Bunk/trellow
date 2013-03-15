using RestSharp;
using trellow.api.Internal;

namespace trellow.api.Boards.Internal
{
	internal class BoardsCloseRequest : BoardsRequest
	{
		public BoardsCloseRequest(IBoardId board)
			: base(board, "closed", Method.PUT)
		{
			this.AddValue("true");			
		}
	}
}