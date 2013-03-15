namespace trellow.api.Members
{
	public class Me : IMemberId
	{
		public string GetMemberId()
		{
			return "me";
		}
	}
}