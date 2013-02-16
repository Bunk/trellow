using TrelloNet.Internal;

namespace TrelloNet
{
	public class Paging
	{
		public Paging(int limit, int page)
		{
			Guard.InRange(limit, 1, MaxLimit, "limit");
			Guard.InRange(page, 0, 100, "page");

			Limit = limit;
			Page = page;
		}

		public int Limit { get; private set; }
		public int Page { get; private set; }

		public static int MaxLimit { get { return 1000; } }
	}
}