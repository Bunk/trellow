using System.Threading.Tasks;

namespace trellow.api.Tokens
{
	public interface IAsyncTokens
	{
		/// <summary>
		/// GET /tokens/[token]
		/// <br/>
		/// Required permissions: read
		/// </summary>
		Task<Token> WithToken(string token);
	}
}