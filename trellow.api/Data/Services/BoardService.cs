using System.Collections.Generic;
using System.Threading.Tasks;
using Strilanc.Value;
using TrelloNet;
using Board = trellow.api.Models.Board;

namespace trellow.api.Data.Services
{
    public class BoardService : ServiceBase, IBoardService
    {
        private ITrello _api;

        public BoardService(IRequestProcessor processor) : base(processor)
        {
        }

        public Task<May<List<Board>>> Mine()
        {
            return Processor.Execute<List<Board>>(
                Request("members/my/boards")
                    .AddParameter("filter", "open"));
        }

        public Task<May<Board>> WithId(string id)
        {
            return Processor.Execute<Board>(
                Request("boards/{id}")
                    .AddUrlSegment("id", id)
                    .AddParameter("lists", "open"));
        }
    }

    public class JsonBoardService : JsonServiceBase, IBoardService
    {
        public async Task<May<List<Board>>> Mine()
        {
            return await ReadFile<List<Board>>("SampleData/boards/boards-mine.json");
        }

        public async Task<May<Board>> WithId(string id)
        {
            var filename = string.Format("SampleData/boards/board-{0}.json", id);
            return await ReadFile<Board>(filename);
        }
    }
}