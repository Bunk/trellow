using System.Collections.Generic;
using System.Threading.Tasks;
using trello.Services.Models;

namespace trello.Services.Data
{
    public class BoardService : ServiceBase, IBoardService
    {
        public BoardService(IRequestProcessor processor) : base(processor)
        {
        }

        public async Task<IEnumerable<Board>> Mine()
        {
            return await Processor.Execute<List<Board>>(
                Request("members/my/boards")
                    .AddParameter("filter", "open"));
        }

        public async Task<Board> WithId(string id)
        {
            return await Processor.Execute<Board>(
                Request("boards/{id}")
                    .AddUrlSegment("id", id)
                    .AddParameter("lists", "open"));
        }
    }
}