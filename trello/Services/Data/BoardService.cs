using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using trello.Services.Models;

namespace trello.Services.Data
{
    public class BoardService : ServiceBase, IBoardService
    {
        public BoardService(IRequestProcessor processor) : base(processor)
        {
        }

        public Task<List<Board>> Mine()
        {
            return Processor.Execute<List<Board>>(
                Request("members/my/boards")
                    .AddParameter("filter", "open"));
        }

        public Task<Board> WithId(string id)
        {
            return Processor.Execute<Board>(
                Request("boards/{id}")
                    .AddUrlSegment("id", id)
                    .AddParameter("lists", "open"));
        }
    }

    public class JsonBoardService : JsonServiceBase, IBoardService
    {
        public Task<List<Board>> Mine()
        {
            return ReadFile<List<Board>>("SampleData/boards/boards-mine.json");
        }

        public Task<Board> WithId(string id)
        {
            var filename = string.Format("SampleData/boards/board-{0}.json", id);
            return ReadFile<Board>(filename);
        }
    }
}