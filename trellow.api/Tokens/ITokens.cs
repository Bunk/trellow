namespace trellow.api.Tokens
{
	public interface ITokens
	{
		/// <summary>
		/// GET /tokens/[token]
		/// <br/>
		/// Required permissions: read
		/// </summary>
		Token WithToken(string token);
	}
}