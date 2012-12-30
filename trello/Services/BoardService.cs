using System.Collections.Generic;
using System.Threading.Tasks;
using trello.Services.Models;

namespace trello.Services
{
    public class BoardService : ServiceBase, IBoardService
    {
        public BoardService(IOAuthClient factory) : base(factory)
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