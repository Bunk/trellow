using System.Threading.Tasks;

namespace TrelloNet.Internal
{
	internal class AsyncTokens : IAsyncTokens
	{
		private readonly IRequestClient _restClient;

		public AsyncTokens(IRequestClient restClient)
		{
			_restClient = restClient;
		}

		public Task<Token> WithToken(string token)
		{
			return _restClient.RequestAsync<Token>(new TokensRequest(token));
		}
	}
}