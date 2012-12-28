using System.Collections.Generic;
using System.Threading.Tasks;

namespace trello.Services
{
    public class BoardService : ServiceBase
    {
        public BoardService(TrelloOAuthClient factory) : base(factory)
        {
        }

        public async Task<IEnumerable<Board>> Mine()
        {
            var request = Request("members/my/boards")
                .AddParameter("filter", "open");

            return await Execute<List<Board>>(request);
        }
    }
}